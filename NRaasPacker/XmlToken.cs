///---------------------------------------------------------------------------
/// File Name:      XmlToken.cs
/// Description:    Represents an Xml token.
/// 
/// Author:         Ali Badereddin
/// Created:        26/12/2009
///---------------------------------------------------------------------------

#region Using Directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

#region Enums

/// <summary>
/// The type of the token can be a special character like < / ? ! >
/// or an escape sequence like &quote; or a comment like <!-- comment -->
/// or an element, attribute, or value.
/// </summary>
public enum XmlTokenType
{
    None,
    SpecialChar,
    Escape,
    Comment,
    Element,
    Attribute,
    Value
}

#endregion

/// <summary>
/// Represents an Xml token with a specific type, index in the parsed string
/// and text.
/// </summary>
public class XmlToken
{
    #region Instance Variables

    private string text;            //  Token Text
    private int index;              //  Token Index
    private XmlTokenType type;      //  Token Type

    #endregion

    #region Constructors

    /// <summary>
    /// Empty Constructor
    /// </summary>
    public XmlToken() { }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="text"></param>
    /// <param name="index"></param>
    /// <param name="type"></param>
    public XmlToken(string text, int index, XmlTokenType type)
    {
        this.text = text;
        this.index = index;
        this.type = type;
    }

    #endregion

    #region Properties

    public string Text
    {
        set
        {
            this.text = value;
        }
        get
        {
            return this.text;
        }
    }

    public int Index
    {
        set
        {
            this.index = value;
        }
        get
        {
            return this.index;
        }
    }

    public XmlTokenType Type
    {
        set
        {
            this.type = value;
        }
        get
        {
            return this.type;
        }
    }

    #endregion
}
