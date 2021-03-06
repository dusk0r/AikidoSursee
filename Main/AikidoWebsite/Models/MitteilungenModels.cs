﻿using System;
using System.Collections.Generic;
using AikidoWebsite.Data.Entities;

namespace AikidoWebsite.Web.Models
{

    public class ListMitteilungenModel
    {
        public int MitteilungenCount { get; set; }
        public IEnumerable<MitteilungModel> Mitteilungen { get; set; }
        public int Start { get; set; }
        public int PerPage { get; set; }
        public bool IsAdmin { get; set; }
    }

    public class ViewMitteilungModel
    {
        public MitteilungModel Mitteilung { get; set; }
        public IEnumerable<DateiModel> Dateien { get; set; } = new List<DateiModel>();
        public IEnumerable<TerminModel> Termine { get; set; } = new List<TerminModel>();
    }

    public class MitteilungModel
    {
        public string Id { get; set; }
        public string Titel { get; set; }
        public DateTime ErstelltAm { get; set; }
        public string AutorId { get; set; }
        public string AutorName { get; set; }
        public string AutorEmail { get; set; }
        public string Text { get; set; }
        public ISet<string> TerminIds { get; set; } = new HashSet<string>();
        public ISet<string> DateiIds { get; set; } = new HashSet<string>();
        public string Html { get; set; }

        public static MitteilungModel Build(Mitteilung mitteilung, Benutzer benutzer = null)
        {
            return new MitteilungModel
            {
                Id = mitteilung.Id,
                Titel = mitteilung.Titel,
                ErstelltAm = mitteilung.ErstelltAm,
                AutorId = mitteilung.AutorId,
                AutorName = benutzer?.Name,
                AutorEmail = benutzer?.EMail,
                Text = mitteilung.Text,
                TerminIds = mitteilung.TerminIds,
                DateiIds = mitteilung.DateiIds,
                Html = mitteilung.Html
            };
        }
    }

    public class EditMitteilungModel
    {
        public MitteilungModel Mitteilung { get; set; }
        public IEnumerable<Termin> Termine { get; set; }
        public IEnumerable<DateiModel> Dateien { get; set; }
        public IList<string> DeletedDateiIds { get; set; }
        public IList<string> DeletedTerminIds { get; set; }

        public EditMitteilungModel()
        {
            this.Mitteilung = new MitteilungModel();
            this.Termine =  new List<Termin>();
            this.Dateien = new List<DateiModel>();
            this.DeletedDateiIds = new List<string>();
            this.DeletedTerminIds = new List<string>();
        }
    }

    public class DateiModel
    {
        public string Id { get; set; }
        public string DateiName { get; set; }
        public string Bezeichnung { get; set; }
        public string ContentType { get; set; }
        public long Size { get; set; }

        public static DateiModel Build(Datei datei)
        {
            return new DateiModel
            {
                Id = datei.Id,
                DateiName = datei.Name,
                Bezeichnung = datei.Beschreibung,
                ContentType = datei.MimeType,
                Size = datei.Bytes
            };
        }
    }

    public class TerminModel
    {
        public string Text { get; set; }
        public DateTime StartDatum { get; set; }
        public DateTime? EndDatum { get; set; }
    }

    public class CreoleModel
    {
        public string Text { get; set; }
    }
}