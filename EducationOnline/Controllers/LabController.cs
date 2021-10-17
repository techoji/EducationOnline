using DistanceLearning.Models;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using File = DistanceLearning.Models.File;
using Rectangle = System.Drawing.Rectangle;

namespace DistanceLearning.Controllers {

    [Authorize]
    public class LabController : Controller {
        private DbManager db { get; set; }
        private const int ZoomArea = 40;

        private Application excel;
        private Workbook book;
        private Workbooks books;
        private _Worksheet sheet;
        private Range xlRange;

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        public LabController() => db = new DbManager();

        [Route("laboratory/tools/{CourseTopicFileId}")]
        public async Task<ActionResult> LabTools(int CourseTopicFileId) {
            CourseTopicFile courseTopicFile = await db.GetCourseTopicFileByIdAsync(CourseTopicFileId);
            File file = await db.GetFileByCourseTopicFileIdAsync(User.Identity.Name, CourseTopicFileId);

            if (courseTopicFile == null ||
                !file.FileType.Name.ToLower().Contains("tools") ||
                !await db.HaveUserAccessToCourseTopicFile(User.Identity.Name, CourseTopicFileId))
                return RedirectToAction("error", "pages");

            bool IsTeacher = await db.IsTeacherByEmailAsync(User.Identity.Name);
            bool IsHaveAccess = false;
            if (IsTeacher) {
                IsHaveAccess = await db.HaveTeacherAccessToCourseByEmailAsync(User.Identity.Name, courseTopicFile.CourseId);
                Session["FileTypes"] = Session["FileTypes"] == null ? db.GetFileTypesLab() : Session["FileTypes"];
            }
            else
                IsHaveAccess = await db.HaveUserAccessToCourseAsync(User.Identity.Name, courseTopicFile.CourseId);
            if (!IsHaveAccess)
                return RedirectToAction("Course", "pages", new { id = courseTopicFile.CourseId });

            FilesLocation location = db.GetFileLocationByFileId(file.FileId);
            string[] FileName = location.FileLocation.Split('\\');

            ViewData["CourseTopicFile"] = courseTopicFile;
            ViewData["FileName"] = FileName[FileName.Length - 1];
            ViewBag.ShowNavigation = false;
            ViewBag.HideSiteLogo = true;

            try {
                excel = new Application();
                books = excel.Workbooks;

                book = books.Open(location.AttachedFileLocation);
                sheet = book.Sheets[1];
                xlRange = sheet.UsedRange;

                int x = GetColumn();
                int y = GetRow();

                List<LabPixelModel> labPixels = new List<LabPixelModel>();
                String[,] sh = new String[y - 1, x];
                for (int i = 2; i <= y; i++) {
                    labPixels.Add(new LabPixelModel() {
                        x1 = (int)xlRange.Cells[i, 1].Value2,
                        y1 = (int)xlRange.Cells[i, 2].Value2,
                        x2 = (int)xlRange.Cells[i, 3].Value2,
                        y2 = (int)xlRange.Cells[i, 4].Value2,
                        L = (double)xlRange.Cells[i, 5].Value2,
                        A = (double)xlRange.Cells[i, 6].Value2,
                        B = (double)xlRange.Cells[i, 7].Value2,
                        V = (double)xlRange.Cells[i, 8].Value2,
                        C = (double)xlRange.Cells[i, 9].Value2,
                        M = (double)xlRange.Cells[i, 10].Value2,
                        Y = (double)xlRange.Cells[i, 11].Value2,
                        Comment = xlRange.Cells[i, 12].Value2.ToString()
                        //Color = xlRange.Cells[i, 13].Value2.ToString()
                    });
                }

                Session["PixelsInfo_" + User.Identity.Name + $"_file{file.FileId}"] = labPixels;
            }
            catch {
                Session["PixelsInfo_" + User.Identity.Name + $"_file{file.FileId}"] = null;
            }

            //Excel close
            books.Close();
            excel.Application.Quit();
            excel.Quit();

            int hWnd = excel.Application.Hwnd;

            GetWindowThreadProcessId((IntPtr)hWnd, out uint processID);
            Process.GetProcessById((int)processID).Kill();

            book = null;
            excel = null;
            sheet = null;

            return View(file);
        }

        [Route("laboratory/zoom/{CourseTopicFileId}")]
        public async Task<ActionResult> LabZoom(int CourseTopicFileId) {
            CourseTopicFile courseTopicFile = await db.GetCourseTopicFileByIdAsync(CourseTopicFileId);
            File file = await db.GetFileByCourseTopicFileIdAsync(User.Identity.Name, CourseTopicFileId);

            if (courseTopicFile == null ||
               !file.FileType.Name.ToLower().Contains("zoom") ||
               !await db.HaveUserAccessToCourseTopicFile(User.Identity.Name, CourseTopicFileId))
                return RedirectToAction("error", "pages");

            bool IsTeacher = await db.IsTeacherByEmailAsync(User.Identity.Name);
            bool IsHaveAccess = false;
            if (IsTeacher) {
                IsHaveAccess = await db.HaveTeacherAccessToCourseByEmailAsync(User.Identity.Name, courseTopicFile.CourseId);
                Session["FileTypes"] = Session["FileTypes"] == null ? db.GetFileTypesLab() : Session["FileTypes"];
            }
            else
                IsHaveAccess = await db.HaveUserAccessToCourseAsync(User.Identity.Name, courseTopicFile.CourseId);
            if (!IsHaveAccess)
                return RedirectToAction("index", "pages");

            FilesLocation location = db.GetFileLocationByFileId(file.FileId);
            bool IsImageCompressed = !String.IsNullOrWhiteSpace(location.AttachedFileLocation);
            String attachedFile = IsImageCompressed ? location.AttachedFileLocation : location.FileLocation;
            String[] FileName = attachedFile.Split('\\');

            if (IsImageCompressed) {
                int OriginalImageWidth = Image.FromStream(new FileStream(location.FileLocation, FileMode.Open, FileAccess.Read)).Width;
                int CompressedImageWidth = Image.FromStream(new FileStream(location.AttachedFileLocation, FileMode.Open, FileAccess.Read)).Width;
                int CompressionRatio = (int)Math.Ceiling(double.Parse(((double)OriginalImageWidth / CompressedImageWidth).ToString()));
                Session["ImageCompressSize_" + User.Identity.Name + file.FileId] = CompressionRatio;
            }
            else
                Session["ImageCompressSize_" + User.Identity.Name + file.FileId] = 1;

            ViewData["FileName"] = FileName[FileName.Length - 1];
            ViewData["CourseTopicFile"] = courseTopicFile;
            ViewData["file"] = file;
            ViewBag.ShowNavigation = false;
            ViewBag.HideSiteLogo = true;

            return View(file);
        }

        public ActionResult GetPixelInfo(int coorX, int coorY, int fileId) {
            List<LabPixelModel> pixelsInfo = Session["PixelsInfo_" + User.Identity.Name + $"_file{fileId}"] as List<LabPixelModel>;
            LabPixelModel pixelInfo = pixelsInfo?.FirstOrDefault(p => p.x1 < coorX && p.x2 > coorX && p.y1 < coorY && p.y2 > coorY);
            pixelInfo = pixelInfo == null ? pixelsInfo == null ? new LabPixelModel() : pixelsInfo[0] : pixelInfo;
            Session["PixelsInfo_" + User.Identity.Name] = pixelsInfo;
            return Json(pixelInfo);
        }

        [Route("laboratory/GetZoomingImage/{coorX}/{coorY}/{fileId}")]
        public FileContentResult GetZoomingImage(int coorX, int coorY, int fileId) {
            using (FileStream fs = new FileStream(db.GetFileLocationByFileId(fileId).FileLocation, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                using (Bitmap image = new Bitmap(fs)) {
                    int CompressionRatio = int.Parse(Session["ImageCompressSize_" + User.Identity.Name + fileId].ToString());
                    int Area = ZoomArea * CompressionRatio;

                    int posX = coorX * CompressionRatio;
                    int posY = coorY * CompressionRatio;

                    int areaWidth = Area > image.Width ? image.Width : Area;
                    int areaHeight = Area > image.Height ? image.Height : Area;

                    int startX = posX - (Area / 2);
                    startX = startX < 0 ? 0 : startX;
                    startX = startX + Area > image.Width ? image.Width - areaWidth : startX;

                    int startY = posY - (Area / 2);
                    startY = startY < 0 ? 0 : startY;
                    startY = startY + Area > image.Height ? image.Height - areaHeight : startY;

                    Rectangle CropArea = new Rectangle(startX, startY, areaWidth, areaHeight);
                    using (Bitmap CropImage = image.Clone(CropArea, image.PixelFormat)) {
                        byte[] bitmapBytes = BitmapToBytes(CropImage);
                        return File(bitmapBytes, "image/jpeg");
                    }
                }
            }
        }

        private static byte[] BitmapToBytes(Bitmap img) {
            using (MemoryStream ms = new MemoryStream()) {
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }

        private int GetColumn() => sheet.Cells.Find("*", System.Reflection.Missing.Value,
                                              System.Reflection.Missing.Value, System.Reflection.Missing.Value,
                                              XlSearchOrder.xlByColumns, XlSearchDirection.xlPrevious,
                                              false, System.Reflection.Missing.Value, System.Reflection.Missing.Value).Column;

        private int GetRow() => sheet.Cells.Find("*", System.Reflection.Missing.Value,
                                            System.Reflection.Missing.Value, System.Reflection.Missing.Value,
                                            XlSearchOrder.xlByRows, XlSearchDirection.xlPrevious,
                                            false, System.Reflection.Missing.Value, System.Reflection.Missing.Value).Row;

        [HttpPost]
        public async Task SetStatisticPerformed(int CourseTopicFileId) => await db.UpdateStatisticPerformedAsync(User.Identity.Name, CourseTopicFileId);
    }
}