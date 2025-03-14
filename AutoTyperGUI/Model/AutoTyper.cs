﻿using System;
using System.Windows.Forms;
using System.Xml;

namespace AutoTyperGUI.Model
{
    /// <summary>
    /// Singleton application controller class
    /// </summary>
    internal class AutoTyper
    {
        /// <summary>
        /// Internal instance of the class (Singleton design pattern)
        /// </summary>
        private readonly static AutoTyper instance = new AutoTyper();

        /// <summary>
        /// Array of the contained chunks (initialised in constructor)
        /// </summary>
        public Chunk[] Chunks { get; private set; }

        /// <summary>
        /// Global typing settings. A refference is assigned to each Chunk (initialized in constructor)
        /// </summary>
        public AutoTypeSettings TypeSettings { get; }

        /// <summary>
        /// Keyboard hook for typing cancellation. Can be assigned by its HotKey property.
        /// </summary>
        public KeyboardHook CancelTypingKHook { get; }

        /// <summary>
        /// Private constructor for singleton implementation
        /// </summary>
        private AutoTyper()
        {
            CancelTypingKHook = new KeyboardHook();
            CancelTypingKHook.KeyPressed += CancelTypingHook_KeyPressed;
            TypeSettings = new AutoTypeSettings();
            initChunks();
        }

        /// <summary>
        /// Property to access the instance of the model (Singleton desing pattern)
        /// </summary>
        public static AutoTyper Instance { get => instance; }

        /// <summary>
        /// Saves the model to a given xml file
        /// </summary>
        /// <param name="fileName">The name of the saved file</param>
        public void save(string fileName)
        {
            XmlTextWriter xmlTextWriter = new XmlTextWriter(fileName, System.Text.Encoding.UTF8);
            xmlTextWriter.Formatting = Formatting.Indented;

            xmlTextWriter.WriteStartDocument();
            {
                xmlTextWriter.WriteStartElement("AutoTyper");
                {

                    foreach (Chunk chunk in Chunks)
                    {
                        xmlTextWriter.WriteStartElement("Chunk");
                        {
                            xmlTextWriter.WriteElementString("Text", chunk.Text);
                            xmlTextWriter.WriteStartElement("ClipboardKHook");
                            {
                                xmlTextWriter.WriteStartElement("HotKey");
                                {
                                    xmlTextWriter.WriteElementString("ModifierKeys", chunk.ClipboardKHook.HotKey.ModifierKeys.Value.ToString());
                                    xmlTextWriter.WriteElementString("Key", ((int)chunk.ClipboardKHook.HotKey.Key).ToString());
                                }
                                xmlTextWriter.WriteEndElement();
                            }
                            xmlTextWriter.WriteEndElement();
                            xmlTextWriter.WriteStartElement("AutoTypeKHook");
                            {
                                xmlTextWriter.WriteStartElement("HotKey");
                                {
                                    xmlTextWriter.WriteElementString("ModifierKeys", chunk.AutoTypeKHook.HotKey.ModifierKeys.Value.ToString());
                                    xmlTextWriter.WriteElementString("Key", ((int)chunk.AutoTypeKHook.HotKey.Key).ToString());
                                }
                                xmlTextWriter.WriteEndElement();
                            }
                            xmlTextWriter.WriteEndElement();
                        }
                        xmlTextWriter.WriteEndElement();
                    }

                    xmlTextWriter.WriteStartElement("Settings");
                    {
                        xmlTextWriter.WriteElementString("TypingSpeed", TypeSettings.TypingSpeed.ToString());
                        xmlTextWriter.WriteElementString("StdDeviation", TypeSettings.StdDeviation.ToString());
                    }
                    xmlTextWriter.WriteEndElement();
                }
                xmlTextWriter.WriteEndElement();
            }
            xmlTextWriter.WriteEndDocument();
            xmlTextWriter.Flush();
            xmlTextWriter.Close();
        }

        /// <summary>
        /// Loads the model contents from an xml file
        /// </summary>
        /// <param name="fileName"></param>
        public void load(string fileName)
        {
            XmlTextReader xmlTextReader = new XmlTextReader(fileName);

            int i = 0;
            KeyboardHook currentHook = null;
            ModifierKeys currentModifierKeys = null;
            Keys currentKey = 0;

            while (xmlTextReader.Read())
            {
                switch (xmlTextReader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (xmlTextReader.Name)
                        {
                            case "Chunk":
                                currentHook = null;
                                currentModifierKeys = null;
                                currentKey = 0;
                                break;
                            case "Text":
                                Chunks[i].Text = xmlTextReader.ReadElementString();
                                break;
                            case "ClipboardKHook":
                                currentHook = Chunks[i].ClipboardKHook;
                                break;
                            case "AutoTypeKHook":
                                currentHook = Chunks[i].AutoTypeKHook;
                                break;
                            case "ModifierKeys":
                                currentModifierKeys = (ModifierKeys)Convert.ToUInt32(xmlTextReader.ReadElementString());
                                break;
                            case "Key":
                                currentKey = (Keys)Convert.ToInt32(xmlTextReader.ReadElementString());
                                break;
                            case "TypingSpeed":
                                TypeSettings.TypingSpeed.CharsPerMin = Convert.ToInt32(xmlTextReader.ReadElementString());
                                break;
                            case "StdDeviation":
                                TypeSettings.StdDeviation = Convert.ToInt32(xmlTextReader.ReadElementString());
                                break;
                        }
                        break;
                    case XmlNodeType.EndElement:
                        switch (xmlTextReader.Name)
                        {
                            case "Chunk":
                                i++;
                                break;
                            case "HotKey":
                                currentHook.HotKey = new HotKey(currentModifierKeys, currentKey);
                                break;
                        }
                        break;

                }
            }
        }

        /// <summary>
        /// Used to initialize the chunks in the default model.
        /// </summary>
        private void initChunks() //Default values are hardcoded
        {
            string[] textChunks;
            HotKey[] clipboardHotKeys;
            HotKey[] autoTypeHotKeys;
            textChunks = new string[12]
            {
                "Text1",
                "Text2",
                "Text3",
                "Text4",
                "Text5",
                "Text6",
                "Text7",
                "Text8",
                "Text9",
                "Text10",
                "Text11",
                "Text12",
            };
            clipboardHotKeys = new HotKey[12]
            {
                new HotKey(ModifierKeys.Control + ModifierKeys.Shift, Keys.F1),
                new HotKey(ModifierKeys.Control + ModifierKeys.Shift, Keys.F2),
                new HotKey(ModifierKeys.Control + ModifierKeys.Shift, Keys.F3),
                new HotKey(ModifierKeys.Control + ModifierKeys.Shift, Keys.F4),
                new HotKey(ModifierKeys.Control + ModifierKeys.Shift, Keys.F5),
                new HotKey(ModifierKeys.Control + ModifierKeys.Shift, Keys.F6),
                new HotKey(ModifierKeys.Control + ModifierKeys.Shift, Keys.F7),
                new HotKey(ModifierKeys.Control + ModifierKeys.Shift, Keys.F8),
                new HotKey(ModifierKeys.Control + ModifierKeys.Shift, Keys.F9),
                new HotKey(ModifierKeys.Control + ModifierKeys.Shift, Keys.F10),
                new HotKey(ModifierKeys.Control + ModifierKeys.Shift, Keys.F11),
                new HotKey(ModifierKeys.Control + ModifierKeys.Shift, Keys.F12),
            };
            autoTypeHotKeys = new HotKey[12]
            {
                new HotKey(ModifierKeys.Control, Keys.F1),
                new HotKey(ModifierKeys.Control, Keys.F2),
                new HotKey(ModifierKeys.Control, Keys.F3),
                new HotKey(ModifierKeys.Control, Keys.F4),
                new HotKey(ModifierKeys.Control, Keys.F5),
                new HotKey(ModifierKeys.Control, Keys.F6),
                new HotKey(ModifierKeys.Control, Keys.F7),
                new HotKey(ModifierKeys.Control, Keys.F8),
                new HotKey(ModifierKeys.Control, Keys.F9),
                new HotKey(ModifierKeys.Control, Keys.F10),
                new HotKey(ModifierKeys.Control, Keys.F11),
                new HotKey(ModifierKeys.Control, Keys.F12)
            };

            
            Chunks = new Chunk[12];
            for (int i = 0; i < 12; i++)
            {
                var clipboardKHook = new KeyboardHook();
                var autoTypeeKHook = new KeyboardHook();
                clipboardKHook.HotKey = clipboardHotKeys[i];
                autoTypeeKHook.HotKey = autoTypeHotKeys[i];
                Chunks[i] = new Chunk(textChunks[i], clipboardKHook, autoTypeeKHook, TypeSettings);
            }
        }

        /// <summary>
        /// Event handler for the CancelTyping keyboard hook
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelTypingHook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            foreach (var chunk in Chunks)
            {
                chunk.CancelTyping();
            }
        }
    }
}
