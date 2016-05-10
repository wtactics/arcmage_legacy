using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using ImageMagick;
using Newtonsoft.Json;
using WTacticsLibrary.Model;

namespace WTacticsLibrary.Layout
{
    public static class DeckGenerator
    {
        
        public static void GenerateDeck(Guid deckGuid, bool generateMissingCards)
        {
            var jsonFile = Repository.GetDeckJsonFile(deckGuid);
            if (!File.Exists(jsonFile)) return;

            var deck = JsonConvert.DeserializeObject<Deck>(File.ReadAllText(jsonFile));

            using (FileStream zipToOpen = new FileStream(Repository.GetDeckZipFile(deckGuid), FileMode.Create))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
                {
                    foreach (var deckCard in deck.DeckCards)
                    {
                        var cardPdf = Repository.GetPdfFile(deckCard.Card.Guid);
                        if (!File.Exists(cardPdf))
                        {
                            if (generateMissingCards)
                            {
                                CardGenerator.CreatePngJob(deckCard.Card.Guid,deckCard.Card.Faction.Name, deckCard.Card.Type.Name);
                            }
                            else
                            {
                                continue;
                            }
                        }

                        var entryName = SanitizeName(deckCard.Card.Name);

                        for (var i = 0; i < deckCard.Quantity; i++)
                        {
                            archive.CreateEntryFromFile(cardPdf, $"{entryName}_{i + 1}.pdf");
                        }
                    }

                    var cardBackPdfFile = Repository.GetBackPdfFile();
                    if (File.Exists(cardBackPdfFile))
                    {
                        archive.CreateEntryFromFile(cardBackPdfFile, $"back.pdf");
                    }

                    var deckFormatFile = Repository.GetDeckFormatFile(deckGuid);
                    if (File.Exists(deckFormatFile))
                    {
                        archive.CreateEntryFromFile(deckFormatFile, $"deck.txt");
                    }

                }
            }

        }

        private static readonly char [] Invalids = System.IO.Path.GetInvalidFileNameChars();

        private static string SanitizeName(string name)
        {
            return string.Join("_", name.Split(Invalids, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
        }


    }


}
