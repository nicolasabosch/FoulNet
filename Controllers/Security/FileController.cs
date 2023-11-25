using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Web;
using System.Collections.Specialized;
using FoulNet.Model;
using System.Net.Http;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Drawing;
using CabernetDBContext;
using Microsoft.AspNetCore.Authorization;

namespace FoulNet.ontrollers
{
    [Route("api/File")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly Entities db;
        public FileController(Entities context)
        {
            db = context;
        }

        // GET api/File
        [HttpGet]
        public ActionResult GetAll()
        {
            NameValueCollection parameters = HttpUtility.ParseQueryString(this.Request.QueryString.Value);
            var languageID = this.CurrentLanguageID();
            var list = from C in db.File select C;
            if (parameters["key"] != null)
            {
                string key = parameters["key"];
                list = list.Where(l => l.FileID == key);
            }



            var ret = list.AsEnumerable();

            return Ok(ret);
        }

        // GET api/Country/5

        [HttpGet("{id}")]
        [AllowAnonymous]
        public ActionResult GetFile(string id)
        {
            var languageID = this.CurrentLanguageID();
            var file = db.File.Find(id);
            string path = Directory.GetCurrentDirectory() + "\\files\\" + file.FolderName + "\\" + file.FileID + "-" + file.FileName;
            var fs = new FileStream(path, FileMode.Open);
            return File(fs, "application/octet-stream", file.FileName);

        }

        // POST api/File
        [HttpPost]
        public ActionResult PostFile()
        {

            if (ModelState.IsValid)
            {
                var postedFile = Request.Form.Files[0];

                var file = new Model.File();
                file.FileID = System.Guid.NewGuid().ToString();
                file.FolderName = System.DateTime.Today.ToString("yyyyMMdd");
                System.IO.Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\files\\" + file.FolderName);
                var filePath = Directory.GetCurrentDirectory() + "\\files\\" + file.FolderName + "\\" + file.FileID + "-" + postedFile.FileName;
                using (var stream = System.IO.File.Create(filePath))
                {
                    postedFile.CopyTo(stream);
                }

                file.FileName = postedFile.FileName;
                file.FileSize = int.Parse(postedFile.Length.ToString());
                db.File.Add(file);
                db.SaveChanges();

                var previewList = new[] { ".jpg", ".png", ".gif", ".bmp" };

                dynamic objFile = new {FileID = file.FileID, 
                FileName = file.FileName,
                Preview = previewList.Contains(file.FileName.ToLower().Substring(file.FileName.ToLower().IndexOf(".")))
                };
                

                //return CreatedAtAction("File", new { id = file.FileID }, objFile);
                return Ok(objFile);

            }
            else
            {
                return BadRequest();
            }

        }



        [HttpGet("GetThumbnailFile/{id}")]
        [AllowAnonymous]
        public ActionResult GetThumbnailFile(string id)
        {
            var file = db.File.Find(id);
            if (file == null)
            {
                return NotFound();
            }

            string path = Directory.GetCurrentDirectory() + "\\files\\" + file.FolderName + "\\" + file.FileID + "-" + file.FileName;


            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                const int MaxDim = 120;

                using (Image originalImage = Image.FromStream(stream))
                {

                    if (originalImage.PropertyIdList.Contains(0x0112))
                    {
                        int rotationValue = originalImage.GetPropertyItem(0x0112).Value[0];
                        switch (rotationValue)
                        {
                            case 1: // landscape, do nothing
                                break;

                            case 8: // rotated 90 right
                                    // de-rotate:
                                originalImage.RotateFlip(rotateFlipType: RotateFlipType.Rotate270FlipNone);
                                break;

                            case 3: // bottoms up
                                originalImage.RotateFlip(rotateFlipType: RotateFlipType.Rotate180FlipNone);
                                break;

                            case 6: // rotated 90 left

                                originalImage.RotateFlip(rotateFlipType: RotateFlipType.Rotate90FlipNone);
                                break;
                        }
                    }

                    int h = originalImage.Height;
                    int w = originalImage.Width;

                    double per = (h > MaxDim) ? (MaxDim * 1.0) / h : 1.0;
                    h = (int)(h * per);
                    w = (int)(w * per);

                    // create the thumbnail image
                    using (Image thumbnailImage = new Bitmap(originalImage, w, h))
                    {
                        var ms = new MemoryStream();
                        thumbnailImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        ms.Position = 0;
                        return File(ms, "image/jpeg", file.FileName);
                    }

                }

            }
        }
    }
}