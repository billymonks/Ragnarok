/*
 * This code is derived from MyJavaLibrary (http://somelinktomycoollibrary)
 * 
 * If this is an open source Java library, include the proper license and copyright attributions here!
 */



namespace com.discobeard.spriter.dom
{
	/// <summary><p>Java class for AnimationObjectRef complex type.</summary>
	/// <remarks>
	/// <p>Java class for AnimationObjectRef complex type.
	/// <p>The following schema fragment specifies the expected content contained within this class.
	/// <pre>
	/// &lt;complexType name="AnimationObjectRef"&gt;
	/// &lt;complexContent&gt;
	/// &lt;restriction base="{http://www.w3.org/2001/XMLSchema}anyType"&gt;
	/// &lt;attribute name="id" type="{http://www.w3.org/2001/XMLSchema}int" /&gt;
	/// &lt;attribute name="timeline" type="{http://www.w3.org/2001/XMLSchema}int" /&gt;
	/// &lt;attribute name="key" type="{http://www.w3.org/2001/XMLSchema}int" /&gt;
	/// &lt;attribute name="z_index" type="{http://www.w3.org/2001/XMLSchema}int" /&gt;
	/// &lt;attribute name="parent" type="{http://www.w3.org/2001/XMLSchema}int" /&gt;
	/// &lt;/restriction&gt;
	/// &lt;/complexContent&gt;
	/// &lt;/complexType&gt;
	/// </pre>
	/// </remarks>
	public class AnimationObjectRef
	{
		protected internal int id;

		protected internal int timeline;

		protected internal int key;

		protected internal int zIndex;

		protected internal int parent;

		//
		// This file was generated by the JavaTM Architecture for XML Binding(JAXB) Reference Implementation, vhudson-jaxb-ri-2.1-2 
		// See <a href="http://java.sun.com/xml/jaxb">http://java.sun.com/xml/jaxb</a> 
		// Any modifications to this file will be lost upon recompilation of the source schema. 
		// Generated on: 2013.01.18 at 06:33:53 PM MEZ 
		//
		/// <summary>Gets the value of the id property.</summary>
		/// <remarks>Gets the value of the id property.</remarks>
		/// <returns>
		/// possible object is
		/// <see cref="int"></see>
		/// </returns>
		public virtual int getId()
		{
			return id;
		}

		/// <summary>Sets the value of the id property.</summary>
		/// <remarks>Sets the value of the id property.</remarks>
		/// <param name="value">
		/// allowed object is
		/// <see cref="int"></see>
		/// </param>
		public virtual void setId(int value)
		{
			this.id = value;
		}

		/// <summary>Gets the value of the timeline property.</summary>
		/// <remarks>Gets the value of the timeline property.</remarks>
		/// <returns>
		/// possible object is
		/// <see cref="int"></see>
		/// </returns>
		public virtual int getTimeline()
		{
			return timeline;
		}

		/// <summary>Sets the value of the timeline property.</summary>
		/// <remarks>Sets the value of the timeline property.</remarks>
		/// <param name="value">
		/// allowed object is
		/// <see cref="int"></see>
		/// </param>
		public virtual void setTimeline(int value)
		{
			this.timeline = value;
		}

		/// <summary>Gets the value of the key property.</summary>
		/// <remarks>Gets the value of the key property.</remarks>
		/// <returns>
		/// possible object is
		/// <see cref="int"></see>
		/// </returns>
		public virtual int getKey()
		{
			return key;
		}

		/// <summary>Sets the value of the key property.</summary>
		/// <remarks>Sets the value of the key property.</remarks>
		/// <param name="value">
		/// allowed object is
		/// <see cref="int"></see>
		/// </param>
		public virtual void setKey(int value)
		{
			this.key = value;
		}

		/// <summary>Gets the value of the zIndex property.</summary>
		/// <remarks>Gets the value of the zIndex property.</remarks>
		/// <returns>
		/// possible object is
		/// <see cref="int"></see>
		/// </returns>
		public virtual int getZIndex()
		{
			return zIndex;
		}

		/// <summary>Sets the value of the zIndex property.</summary>
		/// <remarks>Sets the value of the zIndex property.</remarks>
		/// <param name="value">
		/// allowed object is
		/// <see cref="int"></see>
		/// </param>
		public virtual void setZIndex(int value)
		{
			this.zIndex = value;
		}

		/// <summary>Gets the value of the parent property.</summary>
		/// <remarks>Gets the value of the parent property.</remarks>
		/// <returns>
		/// possible object is
		/// <see cref="int"></see>
		/// </returns>
		public virtual int getParent()
		{
			return parent;
		}

		/// <summary>Sets the value of the parent property.</summary>
		/// <remarks>Sets the value of the parent property.</remarks>
		/// <param name="value">
		/// allowed object is
		/// <see cref="int"></see>
		/// </param>
		public virtual void setParent(int value)
		{
			this.parent = value;
		}
	}
}
