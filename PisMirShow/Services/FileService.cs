using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PisMirShow.Services
{
	public class FileService
	{
		public static byte[] UploadFileInBd(IFormFile file)
		{
			byte[] result;
			using (var ms = new MemoryStream())
			{
				file.CopyTo(ms);
				result = ms.ToArray();
			}

			return result;
		}

		public static string UploadFile(IFormFile file, IHostingEnvironment HostingEnv)
		{
			string result = "";
			string filename = HostingEnv.WebRootPath + $@"\uploadedfiles\{file.FileName}";
			using (FileStream fs = File.Create(filename))
			{
				file.CopyTo(fs);
				result = file.FileName;
				fs.Flush();
			}
			return result;
		}
	}
}
