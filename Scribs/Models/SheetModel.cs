using System.Collections.Generic;
using System.Linq;

namespace Scribs.Models {

    public class SheetTemplateModel {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ProjectKey { get; set; }
        public IDictionary<int, SheetTemplateFieldModel> Fields { get; set; }
        public IList<SheetModel> Sheets { get; set; }
    }

    public class SheetTemplateFieldModel {
        public int Id { get; set; }
        public int Index { get; set; }
        public string Label { get; set; }
    }

    public class SheetModel {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ProjectKey { get; set; }
        public int SheetTemplateId { get; set; }
        public IList<SheetFieldModel> Fields { get; set; }
    }

    public class SheetFieldModel {
        public int Id { get; set; }
        public string Value { get; set; }
        public int SheetTemplateFieldId { get; set; }
    }

    public static class SheetModelUtils {

        public static SheetFieldModel GetModel(SheetField o) => new SheetFieldModel {
            Id = o.Id,
            Value = o.Value,
            SheetTemplateFieldId = o.SheetTemplateFieldId.HasValue ? o.SheetTemplateFieldId.Value : 0
        };

        public static SheetModel GetModel(Sheet o, bool loadFields = false) => new SheetModel {
            Id = o.Id,
            Name = o.Name,
            SheetTemplateId = o.SheetTemplateId,
            Fields = loadFields ? o.SheetFields.Select(t => GetModel(t)).ToList() : null
        };

        public static SheetTemplateFieldModel GetModel(SheetTemplateField o) => new SheetTemplateFieldModel {
            Id = o.Id,
            Label = o.Label
        };

        public static SheetTemplateModel GetModel(SheetTemplate o, bool loadFields = false, bool loadSheets = false, bool loadSheetFields = false) => new SheetTemplateModel {
            Id = o.Id,
            Name = o.Name,
            Fields = loadFields ? o.SheetTemplateFields.ToDictionary(t => t.Id, t => GetModel(t)) : null,
            Sheets = loadSheets ? o.Sheets.Select(t => GetModel(t, loadSheetFields)).ToList() : null
        };
    }
}