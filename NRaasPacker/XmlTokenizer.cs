///---------------------------------------------------------------------------
/// File Name:      XmlTokenzier.cs
/// Description:    Class responsible for splitting Xml text into tokens.
/// 
/// Author:         Ali Badereddin
/// Created:        26/12/2009
///---------------------------------------------------------------------------

#region Using Directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

#endregion

/// <summary>
/// Class responsible for splitting Xml text into tokens.
/// </summary>
public class XmlTokenizer
{
    #region Public Static Methods

    /// <summary>
    /// Split the passed string into Xml tokens.
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    /// <remarks>Looping char by char is more efficient than regular expressions</remarks>
    public static List<XmlToken> Tokenize(string str)
    {
        return LoopTokenize(str);
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Iterate over characters one by one to tokenize the Xml string.
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    private static List<XmlToken> LoopTokenize(string str)
    {
        //  Temp variables to build up the current token
        List<char> currentTokenText = new List<char>();

        //  Represents the list of tokens to be returned
        List<XmlToken> tokens = new List<XmlToken>();

        //  Represents the index of the first character in the token
        int tokenIndex = 0;

        bool isStartTag = false;
        bool isComment = false;
        bool isQuote = false;
        bool isAttribute = false;

        for (int i = 0; i < str.Length; i++)
        {
            //  Get the current character
            char c = str[i];

            //  Skip the "ZERO WIDTH NO-BREAK SPACE" character resulting from encoding
            if (c == 65279)
            {
                continue;
            }

            //  Handle the escape sequence case
            if (c == '&')
            {
                if (currentTokenText.Count > 0)
                {
                    XmlToken token = new XmlToken();
                    token.Index = tokenIndex;
                    token.Text = new string(currentTokenText.ToArray());
                    tokens.Add(token);
                    currentTokenText.Clear();

                    //  Determine token type
                    if (isQuote)
                    {
                        token.Type = XmlTokenType.Value;
                    }
                    else if (isComment)
                    {
                        token.Type = XmlTokenType.Comment;
                    }
                    else if (isStartTag)
                    {
                        if (isAttribute)
                        {
                            token.Type = XmlTokenType.Attribute;
                        }
                        else
                        {
                            token.Type = XmlTokenType.Element;
                        }
                    }
                    else
                    {
                        token.Type = XmlTokenType.None;
                    }
                }

                currentTokenText.Add('&');

                XmlToken escapeToken = new XmlToken();
                escapeToken.Type = XmlTokenType.Escape;
                escapeToken.Index = i;
                i++;

                while (i < str.Length && char.IsLetterOrDigit(str[i]))
                {
                    currentTokenText.Add(str[i]);
                    i++;
                }

                if (i < str.Length && c == ';')
                {
                    currentTokenText.Add(';');
                    i++;
                }

                escapeToken.Text = new string(currentTokenText.ToArray());
                currentTokenText.Clear();
                tokens.Add(escapeToken);
                continue;
            }

            //  Only if the character is not between "" that is in a tag
            if(!isQuote)
            {
                //  Only if the character is not in a comment
                if(!isComment)
                {
                    //  Only if we already have a start tag
                    if(isStartTag)
                    {
                        if (char.IsLetterOrDigit(c))
                        {
                            //  We're starting to build up a token, so save its index
                            if (currentTokenText.Count == 0)
                            {
                                tokenIndex = i;
                            }

                            currentTokenText.Add(c);
                        }
                        else
                        {
                            //  Add the previous token that could be an element or an attribute
                            if (currentTokenText.Count > 0)
                            {
                                XmlToken token = new XmlToken();
                                token.Text = new string(currentTokenText.ToArray());
                                currentTokenText.Clear();
                                token.Index = tokenIndex;

                                if (isAttribute)
                                {
                                    token.Type = XmlTokenType.Attribute;
                                }
                                else
                                {
                                    token.Type = XmlTokenType.Element;
                                }

                                tokens.Add(token);
                            }

                            //  Check if we have something like <!-- to flag that we have a comment
                            if (c == '-')
                            {
                                if (i - 2 >= 0 && i + 1 < str.Length)
                                {
                                    if (str[i - 2] == '<' && str[i - 1] == '!' && str[i + 1] == '-')
                                    {
                                        isStartTag = false;
                                        isComment = true;
                                        i += 1;
                                    }
                                }
                            }
                            //  Check if our start tag is now closed
                            else if (c == '>')
                            {
                                isStartTag = false;
                                isAttribute = false;
                            }
                            //  We hit another start tag
                            else if (c == '<')
                            {
                                isAttribute = false;
                            }
                            //  We're starting a quote
                            else if (c == '"')
                            {
                                isQuote = true;
                            }
                            
                            //  Check if we now have an attribute
                            if (char.IsWhiteSpace(c))
                            {
                                 isAttribute = true;
                            }
                            else
                            {
                                tokens.Add(new XmlToken(c.ToString(), i, XmlTokenType.SpecialChar));
                            }
                        }
                    }
                    //  If we didn't have a start tag, check if we now have one
                    else
                    {
                        if (c == '<')
                        {
                            if (currentTokenText.Count > 0)
                            {
                                XmlToken token = new XmlToken();
                                token.Index = tokenIndex;
                                token.Text = new string(currentTokenText.ToArray());
                                token.Type = XmlTokenType.None;
                                tokens.Add(token);
                                currentTokenText.Clear();
                            }

                            isStartTag = true;
                            tokens.Add(new XmlToken("<", i, XmlTokenType.SpecialChar));
                        }
                        else
                        {
                            if (currentTokenText.Count == 0)
                            {
                                tokenIndex = i;
                            }

                            currentTokenText.Add(c);
                        }
                    }
                }
                //  In case we have a comment
                else
                {
                    //  We're starting to build up a token, so save its index
                    if (currentTokenText.Count == 0)
                    {
                        tokenIndex = i;
                    }

                    currentTokenText.Add(c);

                    //  Check if we have something like --> to see if we're closing a comment
                    //  or if we're at the end
                    if (i + 2 < str.Length)
                    {
                        if (c == '-')
                        {
                            if (str[i + 1] == '-' && str[i + 2] == '>')
                            {
                                isComment = false;
                                i += 2;
                            }
                        }
                    }
                    else
                    {
                        isComment = false;
                    }

                    if (!isComment)
                    {
                        XmlToken token = new XmlToken();
                        token.Type = XmlTokenType.Comment;
                        token.Index = tokenIndex;
                        token.Text = new string(currentTokenText.ToArray());
                        tokens.Add(token);
                        currentTokenText.Clear();
                    }
                }
            }
            //  In case we have a quote
            else
            {
                //  We're starting to build up a token, so save its index
                if (currentTokenText.Count == 0)
                {
                    tokenIndex = i;
                }

                //  Check if we no longer have a quote
                if (c == '"')
                {
                    isQuote = false;
                    XmlToken token = new XmlToken();
                    token.Type = XmlTokenType.Value;
                    token.Index = tokenIndex;
                    token.Text = new string(currentTokenText.ToArray());
                    tokens.Add(token);
                    currentTokenText.Clear();
                }
                else
                {
                    currentTokenText.Add(c);
                }
            }
        }

        //  Handle the last element
        if (currentTokenText.Count > 0)
        {
            XmlToken token = new XmlToken();
            token.Index = tokenIndex;
            token.Text = new string(currentTokenText.ToArray());
            tokens.Add(token);
            currentTokenText.Clear();

            //  Determine token type
            if (isQuote)
            {
                token.Type = XmlTokenType.Value;
            }
            else if (isComment)
            {
                token.Type = XmlTokenType.Comment;
            }
            else if (isStartTag)
            {
                if (isAttribute)
                {
                    token.Type = XmlTokenType.Attribute;
                }
                else
                {
                    token.Type = XmlTokenType.Element;
                }
            }
            else
            {
                token.Type = XmlTokenType.None;
            }
        }

        return tokens;
    }

    #endregion
}
