using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Vanara.PInvoke.Gdi32;
using static Vanara.PInvoke.Shell32;
using Vanara.PInvoke;

namespace electrifier.Controls.Vanara
{
    internal class TempShellUtil
    {
        /// <summary>Gets the size of a bitmap handle.</summary>
        /// <param name="hbmp">The bitmap handle.</param>
        /// <returns>The width of the image referenced by <paramref name="hbmp"/>.</returns>
        public static SIZE GetSize(HBITMAP hbmp)
        {
            var bi = GetObject<BITMAP>(hbmp);
            return new(bi.bmWidth, bi.bmHeight);
        }

		/// <summary>Gets the thumbnail image for the item using the specified characteristics.</summary>
		/// <param name="path">A display name.</param>
		/// <param name="imgSz">The width, in pixels, of the Bitmap.</param>
		/// <param name="flags">One or more of the SIIGBF flags.</param>
		/// <param name="hbmp">The resulting Bitmap, on success, or <c>SafeHBITMAP.Null</c> on failure.</param>
		/// <returns>The result of function.</returns>
		public static HRESULT LoadImageFromImageFactory(string path, ref SIZE imgSz, SIIGBF flags, out SafeHBITMAP hbmp)
		{
			var hr = SHCreateItemFromParsingName(path, null, typeof(IShellItemImageFactory).GUID, out var ppv);
			if (hr.Succeeded)
				return LoadImageFromImageFactory((IShellItemImageFactory)ppv!, ref imgSz, flags, out hbmp);
			hbmp = SafeHBITMAP.Null;
			return hr;
		}

		/// <summary>Gets the thumbnail image for the item using the specified characteristics.</summary>
		/// <param name="pidl">The source PIDL.</param>
		/// <param name="imgSz">The width, in pixels, of the Bitmap.</param>
		/// <param name="flags">One or more of the SIIGBF flags.</param>
		/// <param name="hbmp">The resulting Bitmap, on success, or <c>SafeHBITMAP.Null</c> on failure.</param>
		/// <returns>The result of function.</returns>
		public static HRESULT LoadImageFromImageFactory(PIDL pidl, ref SIZE imgSz, SIIGBF flags, out SafeHBITMAP hbmp)
		{
			var hr = SHCreateItemFromIDList(pidl, typeof(IShellItemImageFactory).GUID, out var ppv);
			if (hr.Succeeded)
				return LoadImageFromImageFactory((IShellItemImageFactory)ppv!, ref imgSz, flags, out hbmp);
			hbmp = SafeHBITMAP.Null;
			return hr;
		}

		/// <summary>Gets the thumbnail image for the item using the specified characteristics.</summary>
		/// <param name="pshiif">The IShellItemImageFactory instance.</param>
		/// <param name="imgSz">The width, in pixels, of the Bitmap.</param>
		/// <param name="flags">One or more of the SIIGBF flags.</param>
		/// <param name="hbmp">The resulting Bitmap, on success, or <c>SafeHBITMAP.Null</c> on failure.</param>
		/// <returns>The result of function.</returns>
		public static HRESULT LoadImageFromImageFactory(IShellItemImageFactory pshiif, ref SIZE imgSz, SIIGBF flags, out SafeHBITMAP hbmp)
		{
			var hr = pshiif.GetImage(imgSz, flags, out hbmp);
			if (hr.Succeeded)
				imgSz = GetSize(hbmp);
			return hr;
		}
    }

    public static class GdiObjExtensions2
    {
        /// <summary>Creates a <see cref="Bitmap"/> from an <see cref="SafeHBITMAP"/> preserving transparency, if possible.</summary>
        /// <param name="hbmp">The SafeHBITMAP value.</param>
        /// <returns>The Bitmap instance. If <paramref name="hbmp"/> is a <c>NULL</c> handle, <see langword="null"/> is returned.</returns>
        public static Bitmap ToBitmap(this SafeHBITMAP hbmp) => ToBitmap((HBITMAP)hbmp);
        //public static Bitmap ToBitmap(this SafeHBITMAP hbmp) => ToBitmap((HBITMAP)hbmp);

        /// <summary>Creates a <see cref="Bitmap"/> from an <see cref="HBITMAP"/> preserving transparency, if possible.</summary>
        /// <param name="hbmp">The HBITMAP value.</param>
        /// <returns>The Bitmap instance. If <paramref name="hbmp"/> is a <c>NULL</c> handle, <see langword="null"/> is returned.</returns>
        public static Bitmap ToBitmap(this in HBITMAP hbmp) => Image.FromHbitmap((IntPtr)hbmp);

    }
}
