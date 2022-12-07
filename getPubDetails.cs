using NXP3_GetPublicationDetails.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace NXP3_GetPublicationDetails
{
    class getPubDetails
    {
        public static string csv_Export_List_Publications_FilePath = @"C:\temp\doc_logs\outputFiles\TDRepo\_Publications\Publication_List.csv";
        //https://docs.rws.com/992513/69978/tridion-docs-14-sp4/publicationoutput-2-5-find
        public static char tab = '\u0009';
        public static string reduceMultiSpace = @"[ ]{2,}";
        public static async Task<List<XmlElement>> Run(InfoShareWSHelper _WSHelper)
        {
            List<XmlElement> sPubSpesificDetails = new List<XmlElement>();
            try
            {
                var pubObj = _WSHelper.GetPublication25Channel();
                string xmlRequestedMetadata = "<ishfields>" +
                        "<ishfield name='FTITLE' level='logical'/>" +
                        "<ishfield name='VERSION' level='version'/>" +
                        "<ishfield name='FISHPUBSOURCELANGUAGES' level='version'/>" +
                        "<ishfield name='FISHMASTERREF' level='version'/>" +
                        "<ishfield name='FISHBASELINE' ishvaluetype='element' level='version'/>" +
                        "<ishfield name='FISHBASELINE' ishvaluetype='value' level='version'/>" +
                        "<ishfield name='FISHRESOURCES' level='version'/>" +
                        "<ishfield name='FISHPUBCONTEXT' level='version'/>" +
                        "<ishfield name='FFSEDOCNUMBER' level='logical'/>" +
                        "<ishfield name='FFSENDA' level='version'/>" +
                        "<ishfield name='FFSESECLEVEL' level='version'/>" +
                    "</ishfields>";
                var PubDetails = pubObj.Find(PublicationOutput25ServiceReference.StatusFilter.ISHNoStatusFilter, null, xmlRequestedMetadata);
                //Console.WriteLine(PubDetails);
                FileStream fs = File.Create(csv_Export_List_Publications_FilePath);
                var newLine = string.Format("Publication GUID\tFTITLE\tVersion\tWorkingLanguage\tFISHMASTERREF\tFISHBASELINE\tFISHRESOURCES\tFISHPUBCONTEXT\tDocumentNum.(FFSEDOCNUMBER)\tNDA(FFSENDA)\tSecurityLevel(FFSESECLEVEL)");
                using (var sr = new StreamWriter(fs))
                {
                    await sr.WriteLineAsync(newLine);
                    sr.Close();
                    sr.Dispose();
                }
                sPubSpesificDetails = writeToFile_csv_Export_List_Publications_FilePath(PubDetails);
            }
            catch (Exception e)
            {

            }
            return sPubSpesificDetails;
        }
        public static List<XmlElement> writeToFile_csv_Export_List_Publications_FilePath(string content)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(content);
            XmlElement root = xDoc.DocumentElement;
            XmlNodeList nodes = root.SelectNodes("ishobject");
            string ftitle = string.Empty;
            string version = string.Empty;
            string fishpubsourcelanguages = string.Empty;
            string fishmasterref = string.Empty;
            string fishbaseline = string.Empty;
            string fishresources = string.Empty;
            string fishpubcontext = string.Empty;
            string ffsedocnumber = string.Empty;//FFSEDOCNUMBER
            string ffsenda = string.Empty;
            string ffseseclevel = "None";
            string logicalID = string.Empty;
            List<XmlElement> PubSpesificDetails = new List<XmlElement>();
            if (nodes != null)
            {
                foreach (XmlElement node in nodes)
                {
                    logicalID = node.Attributes["ishref"].InnerText.ToString();
                    //XmlNode tempNode=node.
                    //string nodeText = node.InnerXml;
                    PubSpesificDetails.Add(node);
                    foreach (XmlElement childNode in node.ChildNodes)
                    {
                        
                        foreach (XmlElement subChildNode in childNode.ChildNodes)
                        {
                            if (subChildNode.Attributes["name"].InnerText.ToString().ToLower().Equals("ftitle"))
                            {
                                ftitle =  subChildNode.InnerText.ToString().Replace(@"\r", "").Replace(@"\n", "").Replace(@"\t", " ").Replace(@"    ", " ");
                                ftitle = Regex.Replace(ftitle.Replace("\t", " "), reduceMultiSpace, " ");
                                ftitle = "\""+string.Join(" ", Regex.Split(ftitle, @"(?:\r\n|\n|\r)"))+ "\"";
                                //if(ftitle.Length>30)
                                //{
                                //    ftitle = ftitle.Substring(0, 29)+"\"";
                                //}
                                //ftitle = ftitle.Replace('.', '_').Replace('[', '_').Replace(']', '_').Replace(':', '_').Replace('~', '_').Replace('!', '_').Replace('@', '_').Replace('#', '_').Replace('$', '_').Replace('%', '_').Replace('%', '_').Replace('^', '_').Replace('&', '_').Replace('*', '_').Replace('-', '_').Replace('\\', '_').Replace('/', '_').Replace('=', '_').Replace('(', '_').Replace(')', '_');
                                //regex
                                //ftitle = "\"" + Regex.Replace(ftitle, @"[^a-zA-Z0-9 ]", "_") + "\"";

                            }
                            if (subChildNode.Attributes["name"].InnerText.ToString().ToLower().Equals("version"))
                            {
                                version = "\"" + subChildNode.InnerText.ToString() + "\"";
                            }
                            if (subChildNode.Attributes["name"].InnerText.ToString().ToLower().Equals("fishpubsourcelanguages"))//FISHPUBSOURCELANGUAGES
                            {
                                fishpubsourcelanguages = "\"" + subChildNode.InnerText.ToString() + "\"";
                            }
                            if (subChildNode.Attributes["name"].InnerText.ToString().ToLower().Equals("fishmasterref"))//FISHMASTERREF
                            {
                                fishmasterref = "\"" + subChildNode.InnerText.ToString() + "\"";
                            }
                            if (subChildNode.Attributes["name"].InnerText.ToString().ToLower().Equals("fishbaseline") && subChildNode.Attributes["ishvaluetype"].InnerText.ToString().ToLower().Equals("value"))//FISHBASELINE
                            {
                                fishbaseline = subChildNode.InnerText.ToString().Replace(@"\r", "").Replace(@"\n", "").Replace(@"\t", " ").Replace(@"	", " ");
                                fishbaseline = Regex.Replace(fishbaseline.Replace("\t", " "), reduceMultiSpace, " ");
                                fishbaseline = "\"" + string.Join(" ", Regex.Split(fishbaseline, @"(?:\r\n|\n|\r)")) + "\"";
                            }
                            if (subChildNode.Attributes["name"].InnerText.ToString().ToLower().Equals("fishresources"))//FISHRESOURCES
                            {
                                fishresources = "\"" + subChildNode.InnerText.ToString() + "\"";
                            }
                            if (subChildNode.Attributes["name"].InnerText.ToString().ToLower().Equals("fishpubcontext"))//FISHPUBCONTEXT
                            {
                                fishpubcontext = subChildNode.InnerText.ToString().Replace("&lt;", "<").Replace("&gt;", ">").Replace("\"","'").Replace(@"\r", "").Replace(@"\n", "").Replace("\t", @" ").Replace(@"	", " ").Replace(@"	", " ");
                                fishpubcontext = Regex.Replace(fishpubcontext.Replace("\t", " "), reduceMultiSpace, " ");

                                fishpubcontext = "\"" + string.Join(" ", Regex.Split(fishpubcontext, @"(?:\r\n|\n|\r)")) + "\"";
                            }
                            if (subChildNode.Attributes["name"].InnerText.ToString().ToLower().Equals("ffsedocnumber"))//doc
                            {
                                ffsedocnumber = "\"" + string.Join(" ", Regex.Split(subChildNode.InnerText.ToString(), @"(?:\r\n|\n|\r)")).Replace(@"\t", " ").Replace(@"	", " ") + "\"";
                                ffsedocnumber = Regex.Replace(ffsedocnumber.Replace("\t", " "), reduceMultiSpace, " ");

                            }
                            if (subChildNode.Attributes["name"].InnerText.ToString().ToLower().Equals("ffsenda"))//FFSENDA
                            {
                                ffsenda = "\"" + string.Join(" ", Regex.Split(subChildNode.InnerText.ToString(), @"(?:\r\n|\n|\r)")).Replace(@"\t", " ").Replace(@" ", " ") + "\"";
                                ffsenda = Regex.Replace(ffsenda.Replace("\t", " "), reduceMultiSpace, " ");

                            }
                            if (subChildNode.Attributes["name"].InnerText.ToString().ToLower().Equals("ffseseclevel"))//FFSESECLEVEL
                            {
                                ffseseclevel = "\"" + string.Join(" ", Regex.Split(subChildNode.InnerText.ToString(), @"(?:\r\n|\n|\r)")).Replace(@"\t", " ").Replace(@"	", " ") + "\"";
                                
                                if(ffseseclevel==("\"\""))
                                {
                                    ffseseclevel = "\"None\"";
                                }
                                ffseseclevel = Regex.Replace(ffseseclevel.Replace("\t", " "), reduceMultiSpace, " ");

                            }
                        }
                    
                    }
                    if(string.IsNullOrEmpty(version))
                    {
                        version = "\"\"";
                    }
                    var newContent = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}", "\"" + logicalID + "\"\t", ftitle+ "\t", version + "\t", fishpubsourcelanguages + "\t", fishmasterref + "\t", fishbaseline + "\t", fishresources + "\t", fishpubcontext + "\t", ffsedocnumber + "\t", ffsenda + "\t", ffseseclevel);
                    if (File.Exists(csv_Export_List_Publications_FilePath))
                    {
                        using (StreamWriter sw = File.AppendText(csv_Export_List_Publications_FilePath))
                        {
                            sw.WriteLine(newContent);
                            sw.Close();
                            sw.Dispose();
                        }
                    }
                }
                
            }
            return PubSpesificDetails;
        }
    }
}
