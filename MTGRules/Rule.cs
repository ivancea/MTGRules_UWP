using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Web.Http;

namespace MTGRules {
    class Rule {
        public string Title { get; set; }
        public string Text { get; set; }

        public List<Rule> SubRules = new List<Rule>();

        public Rule(string title = "", string text = "") {
            this.Title = title;
            this.Text = text;
        }

        public static async Task<List<Rule>> getRules(RulesSource source) {
            string text = await loadRules(source);
            if (text == null)
                return null;
            return textToRules(text);
        }

        private static List<Rule> textToRules(string text)
        {
            List<Rule> rules = new List<Rule>();

            try
            {
                using (StringReader body = new StringReader(text))
                {

                    string t;
                    do
                    {
                        t = body.ReadLine();
                        if (body.Peek() == -1)
                            return null;
                    } while (t != "Credits");

                    int blankLines = 0;
                    while (true)
                    { // Rules
                        t = body.ReadLine();
                        if (t.Length > 0)
                        {
                            if (blankLines > 0 && (t[0] < '1' || t[0] > '9')) // Ended rules
                                break;
                            if ((t.IndexOf(' ') - 1) >= 0 && t[t.IndexOf(' ') - 1] == '.' && t.IndexOf(' ') - 1 == t.IndexOf('.'))
                            {
                                int pos = t.IndexOf(' ');
                                Rule r = new Rule {
                                    Title = t.Substring(0, pos),
                                    Text = t.Substring(pos + 1)
                                };
                                if (t.IndexOf('.') == 1)
                                {
                                    rules.Add(r);
                                }
                                else
                                {
                                    rules.Last().SubRules.Add(r);
                                }
                            }
                            else
                            {
                                if (blankLines == 0)
                                {
                                    rules.Last().SubRules.Last().SubRules.Last().Text += "\n" + t;
                                }
                                else
                                {
                                    int pos = t.IndexOf(' ');
                                    Rule r = new Rule {
                                        Title = t.Substring(0, pos),
                                        Text = t.Substring(pos + 1)
                                    };
                                    rules.Last().SubRules.Last().SubRules.Add(r);
                                }
                            }

                            blankLines = 0;
                        }
                        else
                        {
                            blankLines++;
                        }
                    }

                    Rule glosary = new Rule {Title = "Glosary"};
                    blankLines = 0;
                    string key = "";
                    string value = "";
                    while (true)
                    { // Glosary
                        t = body.ReadLine();

                        if (t.Length > 0)
                        {
                            if (blankLines == 1)
                            {
                                key = t;
                            }
                            else
                            {
                                if (value.Length > 0)
                                    value += "\n";
                                value += t;
                            }
                            blankLines = 0;
                        }
                        else
                        {
                            if (key.Length > 0)
                                glosary.SubRules.Add(new Rule(key, value));
                            key = "";
                            value = "";

                            blankLines++;
                            if (blankLines >= 2)
                                break;
                        }
                    }

                    rules.Add(glosary);

                }
            }
            catch (Exception)
            {
                return null;
            }

            return rules;
        }

        private static async Task<string> loadRules(RulesSource source)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = null;
            if (await localFolder.TryGetItemAsync(source.FileName) != null)
            {
                try {
                    file = await localFolder.GetFileAsync(source.FileName);
                    return await FileIO.ReadTextAsync(file);
                } catch {
                    // ignored
                }
            }

            bool saveToFile = true;
            try
            { // Create file
                file = await localFolder.CreateFileAsync(source.FileName);
            }
            catch
            {
                saveToFile = false;
            }
            
            try
            {
                HttpClient client = new HttpClient();
                var response = await client.SendRequestAsync(new HttpRequestMessage(HttpMethod.Get, source.Uri));
                
                byte[] bodyBytes = (await response.Content.ReadAsBufferAsync()).ToArray();
                Encoding encoding = source.Encoding;

                string rules = encoding.GetString(bodyBytes, 0, bodyBytes.Length);
                
                if (saveToFile)
                {
                    await FileIO.WriteTextAsync(file, rules);
                }
                return rules;
            } catch {
                // ignored
            }

            return null;
        }
    }
}
