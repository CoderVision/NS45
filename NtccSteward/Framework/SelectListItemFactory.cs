using NtccSteward.Core.Models.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NtccSteward.Framework
{
    public static class SelectListItemFactory
    {
        public static List<SelectListItem> Create(IEnumerable<AppEnum> enums, int selectedId, string defaultOptText, string defaultValue)
        {
            var list = new List<SelectListItem>();

            if (!enums.Any(e => e.ID == selectedId))
            {
                list.Add(new SelectListItem() { Text = defaultOptText, Value = defaultValue, Disabled = false, Selected = true });
            }
            
            foreach (var enm in enums)
            {
                bool isSelected = (enm.ID == selectedId);

                list.Add(new SelectListItem() { Text = enm.Desc, Value = enm.ID.ToString(), Disabled = false, Selected = isSelected });
            }

            return list;
        }
    }
}