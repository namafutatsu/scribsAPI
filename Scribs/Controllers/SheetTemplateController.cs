using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Scribs.Models;
using Scribs.Filters;

namespace Scribs.Controllers {

    [JwtAuthentication]
    public class SheetTemplateController : AccessController {

        [HttpPost]
        public async Task<SheetTemplateModel> Post(SheetTemplateModel model) {
            using (var db = new ScribsDbContext()) {
                var user = GetUser(db);
                var template = SheetTemplate.Factory.CreateInstance(db);
                template.ProjectKey = model.ProjectKey;
                template.Name = model.Name;
                foreach (var fieldModel in model.Fields.Values) {
                    CreateField(db, template, fieldModel);
                }
                await db.SaveChangesAsync();
                return SheetModelUtils.GetModel(template, true);
            }
        }

        [HttpPost]
        public async Task<SheetTemplateModel> Update(SheetTemplateModel model) {
            using (var db = new ScribsDbContext()) {
                var user = GetUser(db);
                var template = await SheetTemplate.Factory.GetInstanceAsync(db, model.Id);
                if (template.Name != model.Name)
                    template.Name = model.Name;
                var kept = new HashSet<int>();
                foreach (var kvp in model.Fields.Where(o => o.Key > 0)) {
                    kept.Add(kvp.Key);
                    var field = template.SheetTemplateFields.Single(o => o.Id == kvp.Key);
                    field.Label = kvp.Value.Label;
                    field.Index = kvp.Value.Index;
                }
                foreach (var fieldModel in model.Fields.Where(o => o.Key == 0).Select(o => o.Value)) {
                    CreateField(db, template, fieldModel);
                }
                foreach (var field in template.SheetTemplateFields.Where(o => !kept.Contains(o.Id))) {
                    foreach (var sheet in template.Sheets) {
                        var sheetField = sheet.SheetFields.SingleOrDefault(o => o.SheetTemplateFieldId == field.Id);
                        sheet.SheetFields.Remove(sheetField);
                        SheetField.Factory.Delete(db, sheetField);
                    }
                    template.SheetTemplateFields.Remove(field);
                    SheetTemplateField.Factory.Delete(db, field);
                }
                await db.SaveChangesAsync();
                return SheetModelUtils.GetModel(template, true);
            }
        }

        private void CreateField(ScribsDbContext db, SheetTemplate template, SheetTemplateFieldModel fieldModel) {
            var field = SheetTemplateField.Factory.CreateInstance(db);
            field.Label = fieldModel.Label;
            field.Index = fieldModel.Index;
            template.SheetTemplateFields.Add(field);
        }
    }
}
