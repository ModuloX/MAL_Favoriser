using MarkusRezai.Project.UltraGraph.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MarkusRezai.Project.UltraGraph.Model
{
    public class DetailData : GraphData
    {
        public List<DetailDataField<string>> StringFields { get; set; }
        public List<DetailDataField<long>> LongFields { get; set; }
        public List<DetailDataField<decimal>> DecimalFields { get; set; }
        public List<DetailDataField<bool>> BoolFields { get; set; }
        public List<DetailDataField<DateTime>> DateFields { get; set; }

        public DetailData()
        {
            StringFields = new List<DetailDataField<string>>();
            LongFields = new List<DetailDataField<long>>();
            DecimalFields = new List<DetailDataField<decimal>>();
            BoolFields = new List<DetailDataField<bool>>();
            DateFields = new List<DetailDataField<DateTime>>();
        }

        public DetailData(XElement xSource)
        {
            StringFields = new List<DetailDataField<string>>();
            foreach (XElement xField in xSource.Element("StringFields").Elements("Field")) StringFields.Add(new DetailDataField<string>(xField, xField.Attribute("Value").Value));

            LongFields = new List<DetailDataField<long>>();
            foreach (XElement xField in xSource.Element("LongFields").Elements("Field")) LongFields.Add(new DetailDataField<long>(xField, long.Parse(xField.Attribute("Value").Value)));

            DecimalFields = new List<DetailDataField<decimal>>();
            foreach (XElement xField in xSource.Element("DecimalFields").Elements("Field")) DecimalFields.Add(new DetailDataField<decimal>(xField, decimal.Parse(xField.Attribute("Value").Value)));

            BoolFields = new List<DetailDataField<bool>>();
            foreach (XElement xField in xSource.Element("BoolFields").Elements("Field")) BoolFields.Add(new DetailDataField<bool>(xField, bool.Parse(xField.Attribute("Value").Value)));

            DateFields = new List<DetailDataField<DateTime>>();
            foreach (XElement xField in xSource.Element("DateFields").Elements("Field")) DateFields.Add(new DetailDataField<DateTime>(xField, DateTime.Parse(xField.Attribute("Value").Value)));
        }

        public override XElement ToXElement()
        {
            XElement xStringFields = new XElement("StringFields");
            foreach (DetailDataField<string> field in StringFields) xStringFields.Add(field.ToXElement());

            XElement xLongFields = new XElement("LongFields");
            foreach (DetailDataField<long> field in LongFields) xLongFields.Add(field.ToXElement());

            XElement xDecimalFields = new XElement("DecimalFields");
            foreach (DetailDataField<decimal> field in DecimalFields) xDecimalFields.Add(field.ToXElement());

            XElement xBoolFields = new XElement("BoolFields");
            foreach (DetailDataField<bool> field in BoolFields) xBoolFields.Add(field.ToXElement());

            XElement xDateFields = new XElement("DateFields");
            foreach (DetailDataField<DateTime> field in DateFields) xDateFields.Add(field.ToXElement());

            return new XElement("DetailData",
                                xStringFields,
                                xLongFields,
                                xDecimalFields,
                                xBoolFields,
                                xDateFields
                                );
        }
    }
}
