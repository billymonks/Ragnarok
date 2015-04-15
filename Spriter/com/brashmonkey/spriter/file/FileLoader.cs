/**************************************************************************
 * Copyright 2013 by Trixt0r
 * (https://github.com/Trixt0r, Heinrich Reich, e-mail: trixter16@web.de)
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LobjectCENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS objectS" BASobjectS,
 * WobjectTHOUT WARRANTobjectES OR CONDobjectTobjectONS OF ANY KobjectND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
***************************************************************************/

using System.Collections.Generic;
namespace Com.Brashmonkey.Spriter.file
{
	/// <summary>A FileLoader is an object which takes the task to load the resources a Spriter entity needs.
	/// 	</summary>
	/// <remarks>
	/// A FileLoader is an object which takes the task to load the resources a Spriter entity needs.
	/// This is an abstract implementation and you need to extend this class and specify the logic of
	/// <see cref="FileLoader{object}.load(Reference, string)">FileLoader&lt;object&gt;.load(Reference, string)
	/// 	</see>
	/// .
	/// </remarks>
	/// <author>Trixt0r</author>
	/// <?></?>
	public abstract class FileLoader
	{
		public Dictionary<Reference
			, object> files = new Dictionary<Reference
			, object>();

		public abstract void load(Reference @ref, string path
			);

		/// <summary>objects called if all resources have been passed to this loader.</summary>
		/// <remarks>objects called if all resources have been passed to this loader.</remarks>
		public virtual void finishLoading()
		{
		}

		//To be implemented by your specific backend loader.
		public virtual object get(Reference @ref)
		{
			return files[@ref];
		}

		/// <returns>Array of all loaded references by this loader.</returns>
		public virtual Reference[] getRefs()
		{
			Reference[] refs = new Reference[this.files.Count];
            files.Keys.CopyTo(refs, 0);
			return refs;
		}

		/// <summary>Searches for a reference which is equal to the given one.</summary>
		/// <remarks>
		/// Searches for a reference which is equal to the given one.
		/// Equal means: the folder and file of two references are the same.
		/// </remarks>
		/// <param name="ref">Reference to search after.</param>
		/// <returns>Corresponding reference or null if not found.</returns>
		public virtual Reference findReference(Reference
			 @ref)
		{
			Reference[] refs = this.getRefs();
			foreach (Reference r in refs)
			{
				if (r.Equals(@ref))
				{
					return r;
				}
			}
			return null;
		}

		/// <summary>Searches for all files in the given folder name.</summary>
		/// <remarks>Searches for all files in the given folder name.</remarks>
		/// <param name="folderName">folder to search in</param>
		/// <returns>array of all references which the given folder contains.</returns>
		public virtual Reference[] findReferencesByFolderName(string folderName)
		{
			Reference[] refs = this.getRefs();
			List<Reference> files = new List<Reference>();
			foreach (Reference @ref in refs)
			{
				if (@ref.folderName.Equals(folderName))
				{
					files.Add(@ref);
				}
			}
            refs = new Reference[files.Count];
            files.CopyTo(refs, 0);
            return refs;
		}

		/// <summary>Searches for a reference with the given filename and returns it, if it exists.
		/// 	</summary>
		/// <remarks>Searches for a reference with the given filename and returns it, if it exists.
		/// 	</remarks>
		/// <param name="fileName">name of the file (complete name with folder name and extension)
		/// 	</param>
		/// <returns>reference with given filename or null if not found</returns>
		public virtual Reference findReferenceByFileName(string
			 fileName)
		{
			Reference[] refs = this.getRefs();
			foreach (Reference @ref in refs)
			{
				if (@ref.fileName.Equals(fileName))
				{
					return @ref;
				}
			}
			return null;
		}

		/// <summary>Searches for a reference with the given filename and folder id and returns it, if it exists.
		/// 	</summary>
		/// <remarks>Searches for a reference with the given filename and folder id and returns it, if it exists.
		/// 	</remarks>
		/// <param name="fileName">name of the file (relative name to the given folder)</param>
		/// <param name="folderName">name of the folder in which the file is.</param>
		/// <param name="withoutExtension">indicates whether to compare with the file extension or not. false means, the extension will be compared, too.
		/// 	</param>
		/// <returns>the right reference to the file or null if not found</returns>
		public virtual Reference findReferenceByFileNameAndFolder
			(string fileName, string folderName, bool withoutExtension)
		{
			Reference[] refs = this.findReferencesByFolderName(folderName
				);
			foreach (Reference @ref in refs)
			{
				string file = @ref.fileName.Replace(folderName + "/", string.Empty);
				if (withoutExtension)
				{
					file = file.Replace(".png", string.Empty);
				}
				if (file.Equals(fileName))
				{
					return @ref;
				}
			}
			return null;
		}
	}
}
