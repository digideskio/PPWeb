using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using Newtonsoft.Json.Linq;
using POPpicService.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Http.Tracing;
using System.Web.Http;
using Facebook;
using System.Reflection;
using System.IO;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace POPpicService
{
    public static class Utilities
    {
        public static bool IsRunningLocally() { return true; }

        public static string GetUserId(IPrincipal user)
        {
            // if (IsRunningLocally())
               // return "1605060204";

            var serviceUser = user as ServiceUser;
            if (serviceUser != null)
            {
                var tokens = serviceUser.Id.Split(':');
                return tokens[1].Trim();
            }
            else if (IsRunningLocally())
            {
                return "1605060204";
            }
            else
            {
                return "";
            }
        }

        public static string GetStrippedUserId(IPrincipal user)
        {
            var completeId = GetUserId(user);
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            var trimmed = rgx.Replace(completeId, "");
            return trimmed.ToLower();
        }

        public static FacebookClient InitializeFacebookClient(IPrincipal user, ITraceWriter logger)
        {
            var serviceUser = user as ServiceUser;
            string token = "";
            if (serviceUser != null)
            {
                var claimIdentity = serviceUser.Identity as ClaimsIdentity;
                if (claimIdentity != null)
                {
                    var accessTokenClaim = claimIdentity.Claims.Where(c => c.Value.Contains("accessToken")).FirstOrDefault();
                    if (accessTokenClaim != null)
                    {
                        var accessToken = JObject.Parse(accessTokenClaim.Value)["accessToken"];
                        token = accessToken.Value<string>();
                    }
                }
            }

            token = (string.IsNullOrEmpty(token) && IsRunningLocally()) ? "CAAFEeZBppDVQBACcV7LJCPsHRQKj4SjTQFC8qzl9f4boFZBwWqcZC5hAnZAM2n4ZCmXYbH4GI3k08ZC0LjR6ElIzwXNpr91dNiuYdQGyJa4zY1mG5ZCcrtZAOLZBgVZAMAAVsjv1oDVV9H8ZAZCv3mGqydqUa1FeZADKAOS0jfCLXu2x7yjuuGyQdqVm92RZBcQxJ4UOsZD" : token;
            logger.Info("access token is " + token);

            return string.IsNullOrEmpty(token) ? null : new FacebookClient(token);
        }

        public static POPpicUser InitializeNewUser(IPrincipal user, ITraceWriter logger)
        {
            var client = InitializeFacebookClient(user, logger);

            if (client != null)
            {
                dynamic me = client.Get("me");

                POPpicUser filledInUser = new POPpicUser();
                filledInUser.FacebookId = filledInUser.Id = me.id;
                filledInUser.FirstName = me.first_name;
                filledInUser.LastName = me.last_name;
                filledInUser.ProfilePictureUrl = "https://graph.facebook.com/" + filledInUser.FacebookId + "/picture?type=large";
                filledInUser.LastLatitude = filledInUser.LastLongitude = 0;
                filledInUser.LastSignInTimeStamp = new DateTime(0);
                filledInUser.Loses = filledInUser.Wins = 0;
                filledInUser.UserName = string.Format("{0}.{1}", filledInUser.FirstName, filledInUser.LastName);
                filledInUser.TimePressed = filledInUser.OpponentTimePressed = DateTimeOffset.MinValue;

                return filledInUser;
            }
            else
            {
                return null;
            }
        }


        //The crop image sub
        public static System.Drawing.Image CropImage(System.Drawing.Image Image, int Height, int Width, int StartAtX, int StartAtY)
        {
            Image outimage;
            MemoryStream mm = null;
            try
            {
                //check the image height against our desired image height
                if (Image.Height < Height)
                {
                    Height = Image.Height;
                }

                if (Image.Width < Width)
                {
                    Width = Image.Width;
                }

                //create a bitmap window for cropping
                Bitmap bmPhoto = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
                bmPhoto.SetResolution(72, 72);

                //create a new graphics object from our image and set properties
                Graphics grPhoto = Graphics.FromImage(bmPhoto);
                grPhoto.SmoothingMode = SmoothingMode.AntiAlias;
                grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
                grPhoto.PixelOffsetMode = PixelOffsetMode.HighQuality;

                //now do the crop
                grPhoto.DrawImage(Image, new Rectangle(0, 0, Width, Height), StartAtX, StartAtY, Width, Height, GraphicsUnit.Pixel);

                // Save out to memory and get an image from it to send back out the method.
                mm = new MemoryStream();
                bmPhoto.Save(mm, System.Drawing.Imaging.ImageFormat.Jpeg);
                Image.Dispose();
                bmPhoto.Dispose();
                grPhoto.Dispose();
                outimage = Image.FromStream(mm);

                return outimage;
            }
            catch (Exception ex)
            {
                throw new Exception("Error cropping image, the error was: " + ex.Message);
            }
        }

        //Hard resize attempts to resize as close as it can to the desired size and then crops the excess
        public static Image HardResizeImage(int Width, int Height, System.Drawing.Image Image)
        {
            int width = Image.Width;
            int height = Image.Height;
            Image resized = null;

            if (width > height)
            {
                resized = CropImage(Image, height, height, ((width - height) / 2), 0);
            }
            else
            {
                resized = CropImage(Image, width, width, 0, ((height - width) / 2));
            }

            if (Width > Height)
            {
                resized = ResizeImage(Width, Width, resized);
            }
            else
            {
                resized = ResizeImage(Height, Height, resized);
            }

            return resized;
        }

        //Image resizing
        public static System.Drawing.Image ResizeImage(int maxWidth, int maxHeight, System.Drawing.Image Image)
        {
            int width = Image.Width;
            int height = Image.Height;
            if (width > maxWidth || height > maxHeight)
            {
                //The flips are in here to prevent any embedded image thumbnails -- usually from cameras
                //from displaying as the thumbnail image later, in other words, we want a clean
                //resize, not a grainy one.
                Image.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipX);
                Image.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipX);

                float ratio = 0;
                if (width > height)
                {
                    ratio = (float)width / (float)height;
                    width = maxWidth;
                    height = Convert.ToInt32(Math.Round((float)width / ratio));
                }
                else
                {
                    ratio = (float)height / (float)width;
                    height = maxHeight;
                    width = Convert.ToInt32(Math.Round((float)height / ratio));
                }

                //return the resized image
                return Image.GetThumbnailImage(width, height, null, IntPtr.Zero);
            }
            //return the original resized image
            return Image;
        }

    }
}