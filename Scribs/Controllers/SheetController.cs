using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Scribs.Models;
using Scribs.Filters;

namespace Scribs.Controllers {

    [JwtAuthentication]
    public class SheetController : AccessController {

        [HttpPost]
        public async Task<SheetModel> Post(SheetModel model) {
            using (var db = new ScribsDbContext()) {
                var user = GetUser(db);
                var sheet = Sheet.Factory.CreateInstance(db);
                sheet.ProjectKey = model.ProjectKey;
                sheet.Name = model.Name;
                sheet.SheetTemplateId = model.SheetTemplateId;
                foreach (var fieldModel in model.Fields) {
                    CreateField(db, sheet, fieldModel);
                }
                await db.SaveChangesAsync();
                return SheetModelUtils.GetModel(sheet);
            }
        }

        [HttpPost]
        public async Task<SheetModel> Update(SheetModel model) {
            using (var db = new ScribsDbContext()) {
                var user = GetUser(db);
                var sheet = await Sheet.Factory.GetInstanceAsync(db, model.Id);
                if (sheet.Name != model.Name)
                    sheet.Name = model.Name;
                var kept = new HashSet<int>();
                foreach (var fieldModel in model.Fields.Where(o => o.Id > 0)) {
                    kept.Add(fieldModel.Id);
                    var field = sheet.SheetFields.Single(o => o.Id == fieldModel.Id);
                    field.Value = fieldModel.Value;
                }
                foreach (var fieldModel in model.Fields.Where(o => o.Id == 0)) {
                    CreateField(db, sheet, fieldModel);
                }
                foreach (var field in sheet.SheetFields.Where(o => !kept.Contains(o.Id))) {
                    sheet.SheetFields.Remove(field);
                    SheetField.Factory.Delete(db, field);
                }
                await db.SaveChangesAsync();
                return SheetModelUtils.GetModel(sheet);
            }
        }

        private void CreateField(ScribsDbContext db, Sheet sheet, SheetFieldModel fieldModel) {
            var field = SheetField.Factory.CreateInstance(db);
            field.Value = field.Value;
            field.SheetTemplateFieldId = fieldModel.SheetTemplateFieldId;
            sheet.SheetFields.Add(field);
        }
    }
}
