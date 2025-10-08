using HtmlAgilityPack;

namespace MedXDataCollection
{
    public class HTMLTagDetails
    {
        public string ParentNode { get; set; } = string.Empty;
        public string? BaseNode { get; set; }
        public string? TagName { get; set; }
        public string? TagContent { get; set; }
        public string? TagPropertyList { get; set; }
        public bool CloseTag { get; set; }
    }
    public sealed class HTMLConverter
    {
        private class HTMLTagRecord
        {
            public string Parent_Node { get; set; } = string.Empty;
            public string? Base_Node { get; set; }
            public string? Tag { get; set; }
            public string? Tag_Content { get; set; }
            public string? TagPropertyList { get; set; }
            public bool CloseTag { get; set; }
            public int Addorder { get; set; }
            public bool isInsideFormTag { get; set; }
        }
        private string _url;
        private string _startTag;
        private string _htmlData;        

        public HTMLConverter URL(string url)
        {
            _url = url;
            return this;
        }

        public HTMLConverter SelectTagName(string tagName)
        {
            _startTag = tagName;
            return this;
        }

        public async Task<HTMLConverter> StartProcess()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("_token", "CNNOoAKd6LOuVRgjlbQnS0DI7E0KyTyIZnXe8TPE");
            client.DefaultRequestHeaders.Add("cuf", "MzYxMTg1Mzc3Ng==");
            _htmlData = await client.GetStringAsync(_url);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(_htmlData);

            var templateNode = doc.DocumentNode.SelectSingleNode("//section");
            _htmlData =  templateNode != null ? templateNode.InnerHtml : string.Empty;
            return this;
        }

        public string GetTagStringData()
        {
            return _htmlData;
        }

        public async Task<List<HTMLTagDetails>> GetTagListData()
        {            
            List<HTMLTagDetails> hTMLTagDetails = new List<HTMLTagDetails>();

            List<HTMLTagRecord> hTMLTagRecords = ParseHTMLtoList();

            hTMLTagDetails = hTMLTagRecords.Select(x => new HTMLTagDetails
            {
                ParentNode = x.Parent_Node,
                BaseNode = x.Base_Node,
                TagName = x.Tag,
                TagContent = x.Tag_Content,
                TagPropertyList = x.TagPropertyList,
                CloseTag = x.CloseTag
            }).ToList();

            return hTMLTagDetails;
        }

        private List<HTMLTagRecord> ParseHTMLtoList()
        {
            List<HTMLTagRecord> _listofHTMLTagRecord = new List<HTMLTagRecord>();
            HTMLTagRecord _htmlTagRecord = new HTMLTagRecord();

            var HtmlCode = _htmlData;

            HtmlCode = HtmlCode.Replace("\r\n", "");
            HtmlCode = HtmlCode.Replace("<strong>", "");
            HtmlCode = HtmlCode.Replace("</strong>", "");
            HtmlCode = HtmlCode.Trim();

            char[] HtmlCharArray = HtmlCode.ToCharArray();

            //divide the html code tags into start tag along with its property, content and end tag
            bool isTagContentRead = false;
            bool isStartTagRead = true;
            bool isEndTagRead = false;

            bool isInFromTag = false;

            string contentData = "";
            string startTagData = "";
            string endTagData = "";

            string CurrentParrent = "";
            int addOrder = 1;

            int FindQuate = 0;
            bool IsInsideQuate = false;

            for (int i = 0; i < HtmlCharArray.Length; i++)
            {
                string specialStringforeScapeCondition = "";
                try
                {
                    specialStringforeScapeCondition = (HtmlCharArray[i].ToString() + HtmlCharArray[i + 1].ToString());
                }
                catch (Exception)
                {
                    specialStringforeScapeCondition = HtmlCharArray[i].ToString();
                }

                // their are < or > inside tag content which leads to envoke a if else condition for tag read or content read
                // so we find out < or > is inside a quete or not
                if (FindQuate % 2 == 0 && HtmlCharArray[i] == '"')
                {
                    FindQuate++;
                    IsInsideQuate = true;
                }
                else if (FindQuate % 2 == 1 && HtmlCharArray[i] == '"')
                {
                    FindQuate--;
                    IsInsideQuate = false;
                }

                // if we saw html code tag content start after >
                // before > it could either a start tag or end tag, if it is a end tag then not need to red content
                if (HtmlCharArray[i].ToString() == ">" && specialStringforeScapeCondition != ">>" && IsInsideQuate == false)
                {
                    #region start content read if endtag is null
                    if (string.IsNullOrEmpty(endTagData))
                    {
                        isTagContentRead = true;
                        isStartTagRead = false;
                        isEndTagRead = false;
                        continue;
                    }
                    else
                    {
                        if (endTagData.Replace("/", "").Trim() == "v-form" || endTagData.Replace("/", "").Trim() == "ValidationObserver") // to find is v-col and v-row of vue tag inside form or not 
                        {
                            isInFromTag = false;
                        }
                        isTagContentRead = false;
                        isStartTagRead = false;
                        isEndTagRead = false;
                        endTagData = "";
                        startTagData = "";
                        contentData = "";

                        // update close tag, if current parents got endtag then it is a close tag
                        // so we update the current node as end tag parent node, because its parent node is not close yet
                        foreach (var item in _listofHTMLTagRecord)
                        {
                            if (item.Base_Node == CurrentParrent)
                            {
                                item.CloseTag = true;
                                CurrentParrent = item.Parent_Node;
                                break;
                            }
                        }
                        continue;
                    }
                    #endregion
                }

                // < means it's start of a new tag and end of current tag.....
                // like <div property=" ">Content</div> (strat if new tag)<div></div>
                // also use some sepecial condition for start a new tag or stay into current tag
                // /> is for start a new tag
                // <" or <' is for stay into current tag
                else if ((HtmlCharArray[i].ToString() == "<" && IsInsideQuate == false) || specialStringforeScapeCondition == "/>") // for start a new tag
                {
                    // replace all the special and unwanted charecter
                    //startTagData = startTagData.Replace("<", "").Replace(">", "").Replace("/", "").ToString().Trim();
                    if (!string.IsNullOrEmpty(startTagData))
                    {
                        // a startTagData contain both start tag and tag property...
                        // so we split it in whiteSpace and 1st one is always stat tag
                        var listofProp = startTagData.Split(" ");
                        int index = 0;

                        _htmlTagRecord.Parent_Node = CurrentParrent; // assigning parent....for the very 1st tag no parrent will be available

                        CurrentParrent = Guid.NewGuid().ToString();
                        _htmlTagRecord.Base_Node = CurrentParrent;

                        _htmlTagRecord.Tag = listofProp[0];

                        if (_htmlTagRecord.Tag.Trim() == "v-form" || _htmlTagRecord.Tag.Trim() == "ValidationObserver")
                        {
                            isInFromTag = true; // check if it is a v-form parent tag or not
                        }

                        if (isInFromTag && (_htmlTagRecord.Tag.Trim() != "v-form" || _htmlTagRecord.Tag.Trim() == "ValidationObserver"))
                        {
                            _htmlTagRecord.isInsideFormTag = true;
                        }
                        else
                        {
                            _htmlTagRecord.isInsideFormTag = false;
                        }

                        if (contentData.Trim() == ">>")
                        {
                            _htmlTagRecord.Tag_Content = ">>";
                        }
                        else
                        {
                            _htmlTagRecord.Tag_Content = contentData.Replace(">", "").Replace("<", "").ToString().Trim();
                        }

                        _htmlTagRecord.TagPropertyList = "";
                        foreach (var itm in listofProp)
                        {
                            if (index != 0 && !string.IsNullOrEmpty(itm))
                            {
                                _htmlTagRecord.TagPropertyList += " " + itm.Trim();
                            }
                            index++;
                        }

                        // special condition for close a tag... before close this special one we need to store its data
                        if (specialStringforeScapeCondition == "/>")
                        {
                            _htmlTagRecord.CloseTag = true;
                            CurrentParrent = _htmlTagRecord.Parent_Node;
                        }
                        else
                        {
                            _htmlTagRecord.CloseTag = false;
                        }

                        _htmlTagRecord.Addorder = addOrder; // for getting the adding order of tag
                        addOrder++;

                        _listofHTMLTagRecord.Add(_htmlTagRecord);
                        _htmlTagRecord = new HTMLTagRecord();

                        isTagContentRead = false;
                        isStartTagRead = true;
                        isEndTagRead = false;

                        startTagData = "";
                        contentData = "";
                        endTagData = "";
                    }
                    else
                    {
                        isStartTagRead = true;
                        isTagContentRead = false;
                        isEndTagRead = false;
                    }
                    continue;

                }
                // for closeing tag
                else if (HtmlCharArray[i].ToString() == "/" && IsInsideQuate == false) // for close tag
                {
                    isTagContentRead = false;
                    isStartTagRead = false;
                    isEndTagRead = true;
                    continue;
                }

                if (isTagContentRead || specialStringforeScapeCondition == ">>")
                {
                    contentData += HtmlCharArray[i];
                }

                if (isStartTagRead && specialStringforeScapeCondition != ">>")
                {
                    startTagData += HtmlCharArray[i];
                }

                if (isEndTagRead)
                {
                    endTagData += HtmlCharArray[i];
                }
            }

            return _listofHTMLTagRecord;
        }
    }
}
