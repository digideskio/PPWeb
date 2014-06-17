using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using POPpicService.Models;
using POPpicService.DataObjects;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using ImageResizer;
using System.Drawing.Imaging;

namespace POPpicService.Controllers
{
    public class GamesController : ApiController
    {
        public ApiServices Services { get; set; }

        POPpicContext context;
        protected override void Initialize(System.Web.Http.Controllers.HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            this.context = new POPpicContext();

            
            storageAccount = CloudStorageAccount.Parse(@"DefaultEndpointsProtocol=https;AccountName=poppic;AccountKey=U6MwuYWxKN+pvuE9PFlijRg+iB2Z8VCIV5OAMULjqyzIVTiHa5yzHs8+/dRSgoeqg2xeZdrd/LrJgPUBLSc2GA==");
        }

        private CloudStorageAccount storageAccount;

        [Route("api/Games/myGames")]
        public string GetMyGames()
        {
            DateTimeOffset startingTime = DateTimeOffset.MinValue;
            string userId = Utilities.GetUserId(this.User);

            var myGames = this.context.POPpicGames.Where(g => (g.RequesterId == userId || g.ResponderId == userId)).Select(game => new POPpicGameModel(game)).ToList();
            foreach(var game in myGames)
            {
                game.GameMoves = this.context.POPpicGameMoves.Where(g => g.GameId == game.Id).ToList();
                game.GameResult = this.context.POPpicGameResults.FirstOrDefault(g => g.GameId == game.Id);
            }

            return CrossPlatformUtilities.SerializeItem<List<POPpicGameModel>>(myGames);
        }

        [Route("api/Games/postGameResult")]
        public async Task<string> PostGameResult()
        {
            // Get the game and get the moves
            var gameId = this.ActionContext.Request.Headers.GetValues("GameId").First();
            var gameModel = this.context.POPpicGames.Where(g => g.Id == gameId).FirstOrDefault();
            var gameMoves = this.context.POPpicGameMoves.Where(m => m.GameId == gameId).ToList();
            // var imageStream = await this.ActionContext.Request.Content.ReadAsStreamAsync();

            string fullsizeUrl, thumbnailUrl;
            var urls = await UploadLoserImage(this.ActionContext.Request.Content, gameId);
            fullsizeUrl = urls.Key;
            thumbnailUrl = urls.Value;

            if (gameMoves.Count < 1)
                return "Not enough game moves";

            // Create the result and save it to the DB
            POPpicGameResults result = new POPpicGameResults();
            result.Id = Guid.NewGuid().ToString();
            result.GameId = gameId;
            result.ImageUrl = fullsizeUrl;
            result.LoserId = gameMoves.Last().UserId;
            result.LoserDuration = DurationForPlayer(result.LoserId, gameMoves);
            result.LoserNumberOfMoves = NumberOfMovesForPlayer(result.LoserId, gameMoves);
            result.WinnerId = gameModel.RequesterId == result.LoserId ? gameModel.ResponderId : gameModel.RequesterId;
            result.WinnerDuration = DurationForPlayer(result.WinnerId, gameMoves);
            result.WinnerNumberOfMoves = NumberOfMovesForPlayer(result.WinnerId, gameMoves);
            this.context.POPpicGameResults.Add(result);

            // Next ned to update the players who won/lost
            var winnerModel = this.context.POPpicUsers.Where(u => u.Id == result.WinnerId).First();
            winnerModel.Wins++;
            winnerModel.TimePressed.AddTicks(result.WinnerDuration.Ticks);
            winnerModel.OpponentTimePressed.AddTicks(result.LoserDuration.Ticks);

            var loserModel = this.context.POPpicUsers.Where(u => u.Id == result.LoserId).First();
            loserModel.Loses++;
            loserModel.TimePressed.AddTicks(result.LoserDuration.Ticks);
            loserModel.OpponentTimePressed.AddTicks(result.WinnerDuration.Ticks);
            loserModel.ProfilePictureUrl = thumbnailUrl;

            try
            {
                context.SaveChanges();

            } catch (Exception e)
            {

            }

            // TODO - send push notifications

            return CrossPlatformUtilities.SerializeItem<POPpicGameResults>(result);
        }

        private async Task<KeyValuePair<string, string>> UploadLoserImage(HttpContent content, string gameId)
        {
            string fullsizeUrl = "", thumbnailUrl = "";

            var imageBytes = await content.ReadAsByteArrayAsync();
            var memStream = new MemoryStream(imageBytes);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container. 
            var containerId = Utilities.GetStrippedUserId(this.User);
            CloudBlobContainer container = blobClient.GetContainerReference(containerId);
            container.CreateIfNotExists();
            container.SetPermissions(
                new BlobContainerPermissions
                {
                    PublicAccess =
                        BlobContainerPublicAccessType.Blob
                });

            string blockName = "";
            for (int i = 0; ; i++)
            {
                blockName = gameId + "_" + i.ToString();
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(blockName + ".jpg");
                if (!blockBlob.Exists())
                {
                    blockBlob.UploadFromStream(memStream);
                    fullsizeUrl = blockBlob.Uri.ToString();
                    break;
                }
            }

            // Now we need to resize the image and stuff
            memStream.Seek(0, SeekOrigin.Begin);
            var bmp = Bitmap.FromStream(memStream);
            bmp = Utilities.HardResizeImage(100, 100, bmp);

            Stream thumbnailStream = new MemoryStream();
            bmp.Save(thumbnailStream, ImageFormat.Jpeg);
            thumbnailStream.Seek(0, SeekOrigin.Begin);

            // Save the image
            CloudBlockBlob thumbnailBlob = container.GetBlockBlobReference(blockName + "_thumb.jpg");
            thumbnailBlob.UploadFromStream(thumbnailStream);
            thumbnailUrl = thumbnailBlob.Uri.ToString();

            return new KeyValuePair<string, string>(fullsizeUrl, thumbnailUrl);
        }

        public DateTimeOffset DurationForPlayer(string playerId, IList<POPpicGameMove> gameMoves) {
			DateTimeOffset duration = DateTimeOffset.MinValue;
			var ticks = gameMoves.Where ((POPpicGameMove m) => m.UserId == playerId).Sum ((POPpicGameMove m) => m.MoveDuration.Ticks);
			duration.AddTicks (ticks);
			return duration;
		}

        public int NumberOfMovesForPlayer(string playerId, IList<POPpicGameMove> gameMoves)
        {
            int result;
            result = gameMoves.Where((POPpicGameMove m) => m.UserId == playerId).Count();
            return result;
        }
    }
}
