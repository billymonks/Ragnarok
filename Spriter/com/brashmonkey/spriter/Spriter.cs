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

using Com.Brashmonkey.Spriter.file;
using Com.Brashmonkey.Spriter.xml;
using com.discobeard.spriter.dom;

namespace Com.Brashmonkey.Spriter
{
	/// <summary>This class reads an scml file and loads all necessary resources.</summary>
	/// <remarks>This class reads an scml file and loads all necessary resources.</remarks>
	/// <author>Discobeard.com, Trixt0r</author>
	public class Spriter
	{
		/// <summary>Creates a spriter object.</summary>
		/// <remarks>Creates a spriter object.</remarks>
		/// <param name="path">Path to the scml file</param>
		/// <param name="loader">a loader extended from the AbstractLoader</param>
		/// <returns>a Spriter Object</returns>
		public static Spriter getSpriter(string path, FileLoader loader)
		{
			return new Spriter(path, loader);
		}

		public readonly FileLoader loader;

        public readonly string scmlPath;

		public readonly SpriterData spriterData;

		public Spriter(string scmlPath, FileLoader loader)
		{
            this.scmlPath = scmlPath;
			this.spriterData = Com.Brashmonkey.Spriter.xml.SCMLReader.load(scmlPath);
			this.loader = loader;
			loadResources();
		}

		private void loadResources()
		{
			for (int folder = 0; folder < spriterData.getFolder().Count; folder++)
			{
				for (int file = 0; file < spriterData.getFolder()[folder].getFile().Count; file++)
				{
					string folderName = spriterData.getFolder()[folder].getName();
					string fileName = spriterData.getFolder()[folder].getFile()[file].getName();
					Reference @ref = new Reference
						(folder, file, folderName, fileName);
					@ref.dimensions = new SpriterRectangle(0, spriterData.getFolder
						()[folder].getFile()[file].getHeight(), spriterData.getFolder()[folder].getFile(
						)[file].getWidth(), 0);
					@ref.pivotX = spriterData.getFolder()[folder].getFile()[file].getPivotX();
					@ref.pivotY = spriterData.getFolder()[folder].getFile()[file].getPivotY();
					loader.load(@ref, System.IO.Directory.GetParent(this.scmlPath) + "/" + fileName);
				}
			}
			this.loader.finishLoading();
		}

		/// <returns>Spriter data which has been read from the scml file before.</returns>
		public virtual SpriterData getSpriterData()
		{
			return this.spriterData;
		}
	}
}
