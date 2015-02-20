using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MarkusRezai.Project.UltraGraph.Model
{
    public static class DetailDataTemplateManager
    {
        public static readonly Dictionary<int, DetailDataTemplate> detailDataTemplateDictionary = new Dictionary<int, DetailDataTemplate>();
        public static Dictionary<int, DetailDataTemplate> DetailDataTemplateDictionary { get { return detailDataTemplateDictionary; } }

        public static List<DetailDataTemplate> DetailDataTemplateList { get { return DetailDataTemplateDictionary.Values.ToList(); } }

        public static void LoadDetailDataTepmlates(XElement xSource)
        {
            foreach (XElement xDetailDataTemplate in xSource.Elements("DetailDataTemplate")) 
                DetailDataTemplateDictionary.Add(int.Parse(xDetailDataTemplate.Attribute("Id").Value), new DetailDataTemplate(xDetailDataTemplate));
        }

        public static XElement ToXElement()
        {
            XElement result = new XElement("DetailDataTemplateDictionary");

            foreach (KeyValuePair<int, DetailDataTemplate> detailDataTemplate in DetailDataTemplateDictionary)
            {
                XElement template = detailDataTemplate.Value.ToXElement();
                template.Add(new XAttribute("Id", detailDataTemplate.Key));
                result.Add(template);
            }

            return result;
        }
    }
}
