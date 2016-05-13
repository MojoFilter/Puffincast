using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Band;
using Microsoft.Band.Tiles;
using Microsoft.Band.Tiles.Pages;

using Puffincast.Processing;

namespace Puffincast.Universal
{
    public sealed partial class MainPage : Page
    {
        private HttpQWinampControl control { get; set; }

        public MainPage()
        {
            this.InitializeComponent();

            ISettingsProvider settings = new SettingsProvider();
            control = new HttpQWinampControl(settings);
        }

        public async void ConnectAndSetupBand()
        {
            try
            {
                IBandInfo[] pairedBands = await BandClientManager.Instance.GetBandsAsync();
                IBandInfo band = pairedBands[0];

                using (IBandClient bandClient = await BandClientManager.Instance.ConnectAsync(band))
                {
                    await AddTileIfMissing(bandClient);
                }
            }
            catch (BandException ex)
            {
                var y = ex;
            }
        }

        public async Task AddTileIfMissing(IBandClient bandClient)
        {
            try
            {
                IEnumerable<BandTile> tiles = await bandClient.TileManager.GetTilesAsync();

                var tileToRemove =
                    tiles.Where(p => p.Name == "Puffincast")
                         .Select(p => bandClient.TileManager.RemoveTileAsync(p))
                         .ToList();

                int tileCapacity = await bandClient.TileManager.GetRemainingTileCapacityAsync();

                if (tileCapacity > 0)
                {
                    var smallIconBitmap = new WriteableBitmap(24, 24);
                    var tileIconBitmap = new WriteableBitmap(46, 46);

                    var smallIcon = smallIconBitmap.ToBandIcon();
                    var tileIcon = tileIconBitmap.ToBandIcon();

                    var tileGuid = Guid.NewGuid();
                    var tile = new BandTile(tileGuid)
                    {
                        IsBadgingEnabled = true,
                        Name = "Puffincast",
                        SmallIcon = smallIcon,
                        TileIcon = tileIcon
                    };

                    var panel = new ScrollFlowPanel
                    {
                        Rect = new PageRect(0, 0, 258, 128),
                        Orientation = FlowPanelOrientation.Horizontal,
                        ScrollBarColorSource = ElementColorSource.BandBase,
                    };

                    var textBlock = new Microsoft.Band.Tiles.Pages.TextBlock
                    {
                        ElementId = (short)TileMessagesLayoutElementId.Message1,
                        Rect = new PageRect(0, 0, 258, 128),
                        Margins = new Margins(15, 0, 15, 0),
                        Color = new BandColor(0xFF, 0xFF, 0xFF)
                    };
                    var textBlockData = new TextBlockData((int)TileMessagesLayoutElementId.Message1, "This is a test message");

                    var textButton = new TextButton()
                    {
                        ElementId = 4,
                        HorizontalAlignment = Microsoft.Band.Tiles.Pages.HorizontalAlignment.Center,
                        Rect = new PageRect(0, 0, 100, 50),
                        VerticalAlignment = Microsoft.Band.Tiles.Pages.VerticalAlignment.Center,
                    };

                    var textButtonData = new TextButtonData(4, "Play");

                    panel.Elements.Add(textBlock);
                    panel.Elements.Add(textButton);

                    var layout = new PageLayout(panel);
                    tile.PageLayouts.Add(layout);

                    await bandClient.TileManager.AddTileAsync(tile);

                    var pageContent = new PageData(Guid.NewGuid(), (int)TileLayoutIndex.MessagesLayout, textBlockData, textButtonData);

                    var pageSet = await bandClient.TileManager.SetPagesAsync(tile.TileId, pageContent);
                }
            }
            catch (Exception ex)
            {
                var y = ex;
                throw;
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            ConnectAndSetupBand();
        }

        private async void Previous_Click(object sender, RoutedEventArgs e)
        {
            await control.Prev();
        }

        private async void Play_Click(object sender, RoutedEventArgs e)
        {
            await control.Play();
        }

        private async void Next_Click(object sender, RoutedEventArgs e)
        {
            await control.Next();
        }

        private void Setting_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Puffincast.Universal.Settings));
        }
    }
}

