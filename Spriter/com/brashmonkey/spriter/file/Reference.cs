/**************************************************************************
 * Copyright 2013 by Trixt0r
 * (https://github.com/Trixt0r, Heinrich Reich, e-mail: trixter16@web.de)
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
***************************************************************************/

namespace Com.Brashmonkey.Spriter.file
{
	/// <summary>A Reference is an object which holds a loaded sprite.</summary>
	/// <remarks>A Reference is an object which holds a loaded sprite.</remarks>
	/// <author>Trixt0r</author>
	public class Reference
	{
		public int folder;

		public int file;

		public string folderName;

		public string fileName;

		public Com.Brashmonkey.Spriter.SpriterRectangle dimensions;

		public float pivotX;

		public float pivotY;

		public Reference(int folder, int file, string folderName, string fileName)
		{
			this.folder = folder;
			this.file = file;
			this.folderName = folderName;
			this.fileName = fileName;
		}

		public Reference(int folder, int file) : this(folder, file, null, null)
		{
		}

		public virtual int getFolder()
		{
			return folder;
		}

		public virtual int getFile()
		{
			return file;
		}

		public override int GetHashCode()
		{
			return (folder + "," + file).GetHashCode();
		}

		public override bool Equals(object @ref)
		{
			if (!(@ref is Com.Brashmonkey.Spriter.file.Reference))
			{
				return false;
			}
			//return ((Reference)ref).file == this.file && ((Reference)ref).folder == this.folder;
			return ((Com.Brashmonkey.Spriter.file.Reference)@ref).GetHashCode() == this.GetHashCode
				();
		}
	}
}
