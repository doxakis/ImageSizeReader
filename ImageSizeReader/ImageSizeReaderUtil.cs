using ImageSizeReader.Exceptions;
using ImageSizeReader.Model;
using System;
using System.IO;

namespace ImageSizeReader
{
	public interface IImageSizeReaderUtil
	{
		Size GetDimensions(Stream stream);
	}

	public class ImageSizeReaderUtil : IImageSizeReaderUtil
	{
		public Size GetDimensions(Stream stream)
		{
			using (var binaryReader = new BinaryReader(stream))
			{
				try
				{
					var dimensions = GetDimensions(binaryReader);
					if (dimensions.Width == 0 || dimensions.Height == 0)
						throw new InvalidWidthOrHeightException();

					return dimensions;
				}
				catch (EndOfStreamException)
				{
					throw new CoundNotDetermineDimensionsException(4);
				}
			}
		}

		private Size GetDimensions(BinaryReader binaryReader)
		{
			var byte1 = binaryReader.ReadByte();
			var byte2 = binaryReader.ReadByte();

			if (byte1 == 0xff && byte2 == 0xd8)
				return DecodeJfif(binaryReader);

			if (byte1 == 0x89 && byte2 == 0x50)
			{
				var bytes = binaryReader.ReadBytes(6);
				if (bytes[0] == 0x4E && bytes[1] == 0x47 && bytes[2] == 0x0D && bytes[3] == 0x0A && bytes[4] == 0x1A && bytes[5] == 0x0A)
					return DecodePng(binaryReader);
			}

			if (byte1 == 0x47 && byte2 == 0x49)
			{
				var bytes = binaryReader.ReadBytes(4);
				if (bytes[0] == 0x46 && bytes[1] == 0x38 && (bytes[2] == 0x37 || bytes[2] == 0x39) && bytes[3] == 0x61)
					return DecodeGif(binaryReader);
			}

			throw new UnsupportedFormatException();
		}

		private Size DecodeGif(BinaryReader binaryReader)
		{
			int width = binaryReader.ReadInt16();
			int height = binaryReader.ReadInt16();
			return new Size(width, height);
		}

		private Size DecodePng(BinaryReader binaryReader)
		{
			binaryReader.ReadBytes(8);
			var width = ReadLittleEndianInt32(binaryReader);
			var height = ReadLittleEndianInt32(binaryReader);
			return new Size(width, height);
		}

		private Size DecodeJfif(BinaryReader binaryReader)
		{
			while (binaryReader.ReadByte() == 0xff)
			{
				var marker = binaryReader.ReadByte();
				var chunkLength = ReadLittleEndianInt16(binaryReader);
				if (chunkLength <= 2)
					throw new MalformedImageException();
				
				// 0xda = Start Of Scan
				if (marker == 0xda)
					throw new CoundNotDetermineDimensionsException(1);

				// 0xd9 = End Of Image
				if (marker == 0xd9)
					throw new CoundNotDetermineDimensionsException(2);

				// note: 0xc4 and 0xcc are missing. This is expected.
				if (marker == 0xc0 || marker == 0xc1 || marker == 0xc2 || marker == 0xc3
					|| marker == 0xc5 || marker == 0xc6 || marker == 0xc7 || marker == 0xc8
					|| marker == 0xc9 || marker == 0xca || marker == 0xcb || marker == 0xcd
					|| marker == 0xce || marker == 0xcf
					)
				{
					var precision = binaryReader.ReadByte();
					if (precision == 8 || precision == 12 || precision == 16)
					{
						int height = ReadLittleEndianInt16(binaryReader);
						int width = ReadLittleEndianInt16(binaryReader);
						return new Size(width, height);
					}

					throw new UnexpectedDataPrecisionException(precision);
				}

				// TODO: should perform many time to reduce amount of data being return at once
				binaryReader.ReadBytes(chunkLength - 2);
			}

			throw new CoundNotDetermineDimensionsException(3);
		}

		private int ReadLittleEndianInt16(BinaryReader binaryReader)
		{
			var bytes = new byte[4];
			bytes[1] = binaryReader.ReadByte();
			bytes[0] = binaryReader.ReadByte();
			return BitConverter.ToInt32(bytes, 0);
		}

		private int ReadLittleEndianInt32(BinaryReader binaryReader)
		{
			var bytes = new byte[4];
			bytes[3] = binaryReader.ReadByte();
			bytes[2] = binaryReader.ReadByte();
			bytes[1] = binaryReader.ReadByte();
			bytes[0] = binaryReader.ReadByte();

			return BitConverter.ToInt32(bytes, 0);
		}
	}
}
