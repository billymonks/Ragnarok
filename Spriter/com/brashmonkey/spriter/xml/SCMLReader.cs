/*
 * This code is derived from MyJavaLibrary (http://somelinktomycoollibrary)
 * 
 * If this is an open source Java library, include the proper license and copyright attributions here!
 */


using System.Xml;
using System;
using System.Collections.Generic;
using com.discobeard.spriter.dom;

namespace Com.Brashmonkey.Spriter.xml
{
	/// <summary>This class was implemented to give you the chance loading scml files on android with libGDX since JAXB does not run on android devices.
	/// 	</summary>
	/// <remarks>
	/// This class was implemented to give you the chance loading scml files on android with libGDX since JAXB does not run on android devices.
	/// If you are using libGDX, you should use this class to load scml files.
	/// </remarks>
	/// <author>Trixt0r</author>
	public class SCMLReader
	{
		private static SpriterData data;

		/// <summary>Reads the whole given scml file.</summary>
		/// <remarks>Reads the whole given scml file.</remarks>
		/// <param name="filename">Path to scml file.</param>
		/// <returns>Spriter data in form of lists.</returns>
		public static SpriterData load(string filename)
		{
			try
			{
                System.IO.FileStream stream = new System.IO.FileStream(filename, System.IO.FileMode.Open);
                return load(stream);
			}
			catch (System.IO.FileNotFoundException e)
			{
				//Sharpen.Runtime.PrintStackTrace(e);
                Console.WriteLine(e.StackTrace);
			}
			return null;
		}

        private static SpriterData load(System.IO.FileStream stream)
        {
            XmlReader reader = new XmlReader();
            reader.parse(stream);
            XmlNode root = reader.getNode("spriter_data");
            data = new SpriterData();
            loadFolders(XmlReader.getChildrenByName(root, "folder"));
            loadEntities(XmlReader.getChildrenByName(root, "entity"));
			return data;
		}

		private static void loadFolders(List<XmlNode> folders)
		{
			for (int i = 0; i < folders.Count; i++)
			{
				XmlNode repo = folders[i];
				Folder folder = new Folder(
					);
				folder.setId(XmlReader.getInt(repo,"id" ));

				folder.setName(XmlReader.getAttribute(repo,"name" ));
				List<XmlNode> files = XmlReader.getChildrenByName(repo,"file");
				for (int j = 0; j < files.Count; j++)
				{
					XmlNode f = files[j];
					File file = new File();
					file.setId(XmlReader.getInt(f,"id" ));
					file.setName(XmlReader.getAttribute(f,"name" ));
					file.setWidth(XmlReader.getInt(f,"width" ));
					file.setHeight(XmlReader.getInt(f,"height" ));
					try
					{
						file.setPivotX(XmlReader.getFloat(f,"pivot_x" ));
						file.setPivotY(XmlReader.getFloat(f,"pivot_y" ));
					}
					catch (System.Exception)
					{
						file.setPivotX(System.Convert.ToSingle(0));
						file.setPivotY(System.Convert.ToSingle(1));
					}
					folder.getFile().Add(file);
				}
				data.getFolder().Add(folder);
			}
		}

		private static void loadEntities(List<XmlNode> entities)
		{
			for (int i = 0; i < entities.Count; i++)
			{
				XmlNode e = entities[i];
				Entity entity = new Entity();
				entity.setId(XmlReader.getInt(e,"id" ));
				entity.setName(XmlReader.getAttribute(e,"name" ));
				data.getEntity().Add(entity);
				loadAnimations(XmlReader.getChildrenByName(e,"animation"), entity);
			}
		}

		private static void loadAnimations(List<XmlNode
			> animations, Entity entity)
		{
			for (int i = 0; i < animations.Count; i++)
			{
				XmlNode a = animations[i];
				Animation animation = new Animation
					();
				animation.setId(XmlReader.getInt(a,"id" ));
				animation.setName(XmlReader.getAttribute(a,"name" ));
				animation.setLength(XmlReader.getInt(a,"length" ));
				animation.setLooping(XmlReader.getBool(a,"looping"));
				entity.getAnimation().Add(animation);
				loadMainline(XmlReader.getChildByName(a,"mainline"), animation);
				loadTimelines(XmlReader.getChildrenByName(a,"timeline"), animation);
			}
		}

		private static void loadMainline(XmlNode mainline
			, Animation animation)
		{
			MainLine main = new MainLine
				();
			animation.setMainline(main);
            loadMainlineKeys(XmlReader.getChildrenByName(mainline, "key"), main);
		}

		private static void loadMainlineKeys(List<XmlNode
			> keys, MainLine main)
		{
			for (int i = 0; i < keys.Count; i++)
			{
				XmlNode k = keys[i];
				Key key = new Key();
				key.setId(XmlReader.getInt(k,"id" ));
                int time = XmlReader.getInt(k, "time", -1);
                if (time == -1) key.setTime(0);
                else key.setTime(System.Convert.ToInt64(time));
				main.getKey().Add(key);
                loadRefs(XmlReader.getChildrenByName(k, "object_ref"), XmlReader.getChildrenByName(k, "bone_ref"), key);
			}
		}

		private static void loadRefs(List<XmlNode
			> objectRefs, List<XmlNode
			> boneRefs, Key key)
		{
			for (int i = 0; i < boneRefs.Count; i++)
			{
				XmlNode o = boneRefs[i];
				BoneRef bone = new BoneRef(
					);
				bone.setId(XmlReader.getInt(o,"id" ));
				bone.setKey(XmlReader.getInt(o,"key" ));
                int par = XmlReader.getInt(o, "parent", -1);
                bone.setParent(par);
				bone.setTimeline(XmlReader.getInt(o,"timeline" ));
				key.getBoneRef().Add(bone);
			}
			for (int i_1 = 0; i_1 < objectRefs.Count; i_1++)
			{
				XmlNode o = objectRefs[i_1];
				AnimationObjectRef @object = new AnimationObjectRef
					();
				@object.setId(XmlReader.getInt(o,"id" ));
				@object.setKey(XmlReader.getInt(o,"key" ));
                int par = XmlReader.getInt(o, "parent", -1);
				@object.setParent(par);
				@object.setTimeline(XmlReader.getInt(o,"timeline" ));
				@object.setZIndex(XmlReader.getInt(o,"z_index" ));
				key.getObjectRef().Add(@object);
			}
		}

		private static void loadTimelines(List<XmlNode
			> timelines, Animation animation)
		{
			for (int i = 0; i < timelines.Count; i++)
			{
				TimeLine timeline = new TimeLine
					();
				timeline.setId(XmlReader.getInt(timelines[i],"id" ));
				animation.getTimeline().Add(timeline);
				loadTimelineKeys(XmlReader.getChildrenByName(timelines[i],"key"), timeline);
			}
		}

		private static void loadTimelineKeys(List<XmlNode> keys, 
            TimeLine timeline)
		{
			for (int i = 0; i < keys.Count; i++)
			{
				XmlNode k = keys[i];
                XmlNode obj = XmlReader.getChildByName(k, "bone");
				Key key = new Key();
				key.setId(XmlReader.getInt(k,"id" ));
				key.setSpin(XmlReader.getInt(k,"spin", 1));
				key.setTime(System.Convert.ToInt64(XmlReader.getInt(k,"time", 0 )));
                string name = XmlReader.getAttribute(k.ParentNode, "name");
				timeline.setName(name);
				if (obj != null)
				{
					Bone bone = new Bone();
					bone.setAngle(XmlReader.getFloat(obj,"angle", 0f ));
					bone.setX(XmlReader.getFloat(obj,"x", 0f ));
					bone.setY(XmlReader.getFloat(obj,"y", 0f ));
					bone.setScaleX(XmlReader.getFloat(obj,"scale_x", 1f ));
					bone.setScaleY(XmlReader.getFloat(obj,"scale_y", 1f ));
					key.setBone(bone);
				}
				else
				{
					AnimationObject @object = new AnimationObject
						();
                    obj = XmlReader.getChildByName(k, "object");
					@object.setAngle(XmlReader.getFloat(obj,"angle", 0f ));
					@object.setX(XmlReader.getFloat(obj,"x", 0f ));
					@object.setY(XmlReader.getFloat(obj,"y", 0f ));
					@object.setScaleX(XmlReader.getFloat(obj,"scale_x", 1f ));
					@object.setScaleY(XmlReader.getFloat(obj,"scale_y", 1f ));
					@object.setFolder(XmlReader.getInt(obj,"folder" ));
					@object.setFile(XmlReader.getInt(obj,"file" ));
					File f = data.getFolder()[@object.getFolder()].getFile
						()[@object.getFile()];
					@object.setPivotX(XmlReader.getFloat(obj,"pivot_x", f.getPivotX()));
					@object.setPivotY(XmlReader.getFloat(obj,"pivot_y", f.getPivotY()));
					key.getObject().Add(@object);
				}
				timeline.getKey().Add(key);
			}
		}

		public virtual SpriterData getCurrentSpriterData()
		{
			return data;
		}
	}
}
