///---------------------------------------------------------------------------
/// File Name:      XmlEditor.cs
/// Description:    Editor that does Xml formatting and syntax highlighting.
/// 
/// Author:         Ali Badereddin
/// Created:        26/12/2009
///---------------------------------------------------------------------------

#region Using Directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

#endregion

/// <summary>
/// Editor that does Xml formatting and syntax highlighting.
/// </summary>
public partial class XmlEditor : UserControl
{
    #region Instance Variables

    private static Color specialCharColor = Color.Blue;   //  Color for special characters
    private static Color escapeColor = Color.Orchid;      //  Color for escape sequences
    private static Color elementColor = Color.DarkRed;    //  Color for Xml elements
    private static Color attributeColor = Color.Red;      //  Color for Xml attributes
    private static Color valueColor = Color.DarkBlue;     //  Color for attribute values
    private static Color commentColor = Color.DarkGreen;  //  Color for Xml comments

    private bool allowXmlFormatting = true;               //  Whether to do Xml formatting when text changes

    #endregion

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    public XmlEditor()
    {
        InitializeComponent();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Set or get the text of the xml editor.
    /// </summary>
    public override string Text
    {
        set
        {
            xmlTextBox.Text = value;
        }
        get
        {
            return xmlTextBox.Text;
        }
    }

    /// <summary>
    /// Tells whether to format the editor's Xml or not.
    /// </summary>
    public bool AllowXmlFormatting
    {
        set
        {
            allowXmlFormatting = value;

            if (allowXmlFormatting)
            {
                FormatXml(xmlTextBox);
            }
        }
        get
        {
            return allowXmlFormatting;
        }
    }

    /// <summary>
    /// Whether to allow the user to change text.
    /// </summary>
    public bool ReadOnly
    {
        set
        {
            this.xmlTextBox.ReadOnly = value;
        }
        get
        {
            return this.xmlTextBox.ReadOnly;
        }
    }

    #endregion

    #region UI Events

    /// <summary>
    /// Format Xml when text changes.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void xmlTextBox_TextChanged(object sender, EventArgs e)
    {
        if (allowXmlFormatting)
        {
            FormatXml(xmlTextBox);
        }

        NRaasPacker.MainForm.PreviewChanged = true;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Format Xml in the passed rich text box.
    /// </summary>
    /// <param name="xmlEditor"></param>
    public static void FormatXml(RichTextBox xmlEditor)
    {
        //  Stop redrawing
        RichTextDrawing.StopRedraw(xmlEditor);

        //  Tokenize the Xml string
        List<XmlToken> tokens = XmlTokenizer.Tokenize(xmlEditor.Text);
        foreach (XmlToken token in tokens)
        {
            xmlEditor.Select(token.Index, token.Text.Length);
            switch (token.Type)
            {
                case XmlTokenType.None:
                    xmlEditor.SelectionColor = xmlEditor.ForeColor;
                    break;
                case XmlTokenType.SpecialChar:
                    xmlEditor.SelectionColor = specialCharColor;
                    break;
                case XmlTokenType.Escape:
                    xmlEditor.SelectionColor = escapeColor;
                    break;
                case XmlTokenType.Element:
                    xmlEditor.SelectionColor = elementColor;
                    break;
                case XmlTokenType.Attribute:
                    xmlEditor.SelectionColor = attributeColor;
                    break;
                case XmlTokenType.Value:
                    xmlEditor.SelectionColor = valueColor;
                    break;
                case XmlTokenType.Comment:
                    xmlEditor.SelectionColor = commentColor;
                    break;
            }
        }

        //  Sample code to show that the perf problem is a RichTexBox problem
        //string content = xmlEditor.Text;
        //Random gen = new Random();
        //for (int i = 0; i < content.Length; i++)
        //{
        //    xmlEditor.Select(i, 1);
        //    Color c = Color.FromArgb(gen.Next(256), gen.Next(256), gen.Next(256));
        //    xmlEditor.SelectionColor = c;
        //}

        //  Resume redraw
        RichTextDrawing.RestoreRedraw(xmlEditor);
    }

    #endregion

}

#region Helper Class

/// <summary>
/// Helper class to change colors on a RichTextBox without flickering.
/// </summary>
public class RichTextDrawing
{
    private static int lastSelection;

    [DllImport("user32.dll")]
    public static extern bool LockWindowUpdate(IntPtr hWndLock);

    public static void StopRedraw(RichTextBox richTextBox)
    {
        LockWindowUpdate(richTextBox.Handle);

        //  Save the last location 
        lastSelection = richTextBox.SelectionStart;

        // Refresh colors
        richTextBox.SelectAll();
        richTextBox.SelectionColor = richTextBox.ForeColor;
    }

    public static void RestoreRedraw(RichTextBox richTextBox)
    {
        LockWindowUpdate(IntPtr.Zero);

        //  Restore selection and color state
        richTextBox.SelectionStart = lastSelection;
        richTextBox.SelectionLength = 0;
        richTextBox.SelectionColor = richTextBox.ForeColor;
    }
}

#endregion