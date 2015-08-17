﻿using MusCatalog.Model;
using System;
using System.Linq;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using Microsoft.Win32;

namespace MusCatalog.View
{
    /// <summary>
    /// Interaction logic for EditAlbumWindow.xaml
    /// </summary>
    public partial class EditAlbumWindow : Window
    {
        private Album album = null;

        // Song list
        List<Song> albumSongs = new List<Song>();

        // bitmaps for stars
        BitmapImage imageStar = App.Current.TryFindResource("ImageStar") as BitmapImage;
        BitmapImage imageHalfStar = App.Current.TryFindResource("ImageHalfStar") as BitmapImage;
        BitmapImage imageEmptyStar = App.Current.TryFindResource("ImageEmptyStar") as BitmapImage;

        // number of the star that was clicked
        byte starPos = 0;


        public EditAlbumWindow( Album a )
        {
            InitializeComponent();

            // save current album in 'album' variable
            album = a;
            
            // load and prepare all songs from the album for further actions
            using (var context = new MusCatEntities())
            {
                albumSongs = context.Songs.Where(s => s.Album.ID == a.ID).ToList();

                foreach (var song in albumSongs)
                {
                    song.Album = album;
                }

                this.rateAlbum.DataContext = a;
                this.AlbumInfoPanel.DataContext = a;
                this.GridSongs.ItemsSource = albumSongs;    
            }
        }

        private void ParseMp3(object sender, RoutedEventArgs e)
        {
            var fbd = new System.Windows.Forms.FolderBrowserDialog();
            if (fbd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            var i = 0;
            foreach ( var filename in Directory.GetFiles( fbd.SelectedPath, "*.mp3" ) )
            {
                if (i == albumSongs.Count)
                {
                    albumSongs.Add(new Song { ID = -1, AlbumID = album.ID });
                }

                var file = TagLib.File.Create(filename);
                var v2tag = (TagLib.Id3v2.Tag)file.GetTag(TagLib.TagTypes.Id3v2);

                if (v2tag != null)
                {
                    albumSongs[i].Name = v2tag.Title;
                }
                else
                {
                    TagLib.Id3v1.Tag v1tag;
                    v1tag = (TagLib.Id3v1.Tag)file.GetTag(TagLib.TagTypes.Id3v1);
                    if (v1tag != null)
                    {
                        albumSongs[i].Name = v1tag.Title;
                    }
                    else
                    {
                        albumSongs[i].Name = Path.GetFileNameWithoutExtension(filename);
                    }
                }

                albumSongs[i].TrackNo = (byte)(i + 1);
                albumSongs[i].TimeLength = file.Properties.Duration.ToString(@"m\:ss");

                file.Dispose();

                i++;
            }

            FixTimes(null, null);

            this.GridSongs.Items.Refresh();
        }

        private void SaveSongCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        private void SaveSong(object sender, ExecutedRoutedEventArgs e)
        {
            var song = this.GridSongs.SelectedItem as Song;
            if (song != null)
            {
                using (var context = new MusCatEntities())
                {
                    context.Entry(context.Songs.Find(song.ID)).CurrentValues.SetValues(song);
                    context.SaveChanges();
                }
            }
        }

        private void DeleteSong(object sender, ExecutedRoutedEventArgs e)
        {
            var b = sender as Button;
            MessageBox.Show(b.CommandParameter + "");
        }

        private void AddSong(object sender, RoutedEventArgs e)
        {
            //if ( this.GridSongs.Items.Count > 0 && albumSongs.Last().ID == -1 )
            //{
            //    return;
            //}

            byte newTrackNo = (byte)(albumSongs.Last().TrackNo + 1);
            albumSongs.Add( new Song { ID = -1, TrackNo = newTrackNo, AlbumID = album.ID, Album = album} );
            
            this.GridSongs.ItemsSource = albumSongs;
            this.GridSongs.Items.Refresh();
        }

        private void SaveAlbumInformation(object sender, RoutedEventArgs e)
        {
            using (var context = new MusCatEntities())
            {
                context.Entry(album).State = System.Data.EntityState.Modified;
                context.SaveChanges();
            }
        }

        private void FixNames(object sender, RoutedEventArgs e)
        {
            foreach (var s in albumSongs)
            {
                s.Name = s.Name.Trim();
                s.Name = s.Name.Replace("_", " ");
                s.Name = s.Name.ToLower();
                
                string oldLetter = s.Name.Substring(0,1);
                s.Name = s.Name.Remove(0, 1).Insert(0, oldLetter.ToUpper());

                int spacePos = s.Name.IndexOf(' ');
                while ( spacePos > -1 )
                {
                    oldLetter = s.Name.Substring(spacePos + 1, 1);
                    s.Name = s.Name.Remove(spacePos + 1, 1).Insert(spacePos + 1, oldLetter.ToUpper());
                    spacePos = s.Name.IndexOf(' ', spacePos + 1);
                }
            }

            this.GridSongs.Items.Refresh();
        }

        private void FixTimes(object sender, RoutedEventArgs e)
        {
            int totalMinutes = 0, totalSeconds = 0;

            foreach (var s in albumSongs)
            {
                // fix each record if there's a need
                string clean = "";
                foreach (char c in s.TimeLength)
                {
                    if ( char.IsDigit(c) || c == ':')
                        clean += c;
                }

                int colonPos = clean.IndexOf(':');
                
                if (colonPos == -1)
                {
                    clean = clean + ":00";
                }
                else if (clean.Length - colonPos < 2)
                {
                    clean = clean.Insert( colonPos + 1, "0" );
                }

                s.TimeLength = clean;
                colonPos = clean.IndexOf(':');

                totalMinutes += int.Parse( s.TimeLength.Substring(0, colonPos) );
                totalSeconds += int.Parse( s.TimeLength.Substring(colonPos + 1) );
            }
            
            // calculate total time
            totalMinutes += totalSeconds / 60;
            totalSeconds = totalSeconds % 60;

            album.TotalTime = string.Format("{0}:{1:00}", totalMinutes, totalSeconds);
            this.TotalTime.Text = album.TotalTime;
        }

        private void ClearAll(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(string.Format("Are you sure you want to delete all songs in the album\n '{0}' \nby '{1}'?",
                                            album.Name, album.Performer.Name),
                                            "Confirmation",
                                            MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                albumSongs.Clear();
                this.GridSongs.Items.Refresh();
            }
        }

        private void SaveAll(object sender, RoutedEventArgs e)
        {
            using (var context = new MusCatEntities())
            {
                foreach (var song in albumSongs)
                {
                    if (song.ID == -1)
                    {
                        song.ID = context.Songs.Max(s => s.ID) + 1;
                        context.Songs.Add( song );
                    }
                    else
                    {
                        context.Entry(context.Songs.Find(song.ID)).CurrentValues.SetValues(song);
                    }
                    context.SaveChanges();
                }
            }
        }

        private void LoadAlbumImageFromClipboard(object sender, RoutedEventArgs e)
        {
            if (!Clipboard.ContainsImage())
            {
                MessageBox.Show("No image in clipboard!");
                return;
            }

            var filepaths = FileLocator.MakePathImageAlbum(album);
            string filepath = filepaths[0];

            if (filepaths.Count > 1)
            {
                ChoiceWindow choice = new ChoiceWindow();
                choice.SetChoiceList(filepaths);
                choice.ShowDialog();

                if (choice.ChoiceResult == "")
                {
                    return;
                }
                filepath = choice.ChoiceResult;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(filepath));

            var image = Clipboard.GetImage();
            try
            {
                // first check if file already exists
                if (File.Exists(filepath))
                {
                    File.Delete(filepath);
                }

                using (var fileStream = new FileStream(filepath, FileMode.CreateNew))
                {
                    BitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(image));
                    encoder.Save(fileStream);

                    this.AlbumCover.Source = encoder.Frames[0];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadAlbumImageFromFile(object sender, RoutedEventArgs e)
        {

        }

        
        #region Star image methods: mouse over, mouse leave, mouse down

        /// <summary>
        /// 
        /// </summary>
        private void StarMouseMove(object sender, MouseEventArgs e)
        {
            starPos = 0;

            // loop to find out what star has triggered the MouseMove event
            foreach (var elem in this.rateAlbum.Children)
            {
                starPos++;

                if (elem == (Image)sender)
                    break;
            }

            // draw all stars to the left as "full" stars
            for (int i = 0; i < starPos - 1; i++)
            {
                ((Image)this.rateAlbum.Children[i]).Source = imageStar;
            }

            // if the X coordinate of mouse position exceeds the half of a star size
            if (e.GetPosition((Image)sender).X > 25 / 2)
            {
                // then draw full star
                ((Image)sender).Source = imageStar;
            }
            else
            {
                // else draw half of the star
                ((Image)sender).Source = imageHalfStar;
            }

            // rest of the stars are empty
            for (int i = starPos; i < 5; i++)
            {
                ((Image)this.rateAlbum.Children[i]).Source = imageEmptyStar;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void StarMouseLeave(object sender, MouseEventArgs e)
        {
            starPos = 0;
            for (int i = 0; i < 5; i++)
            {
                ((Image)this.rateAlbum.Children[i]).Source = imageEmptyStar;
            }

            if (album.Rate.HasValue)
            {
                int i = 0;
                for (; i < album.Rate / 2; i++)
                {
                    ((Image)this.rateAlbum.Children[i]).Source = imageStar;
                }

                if (album.Rate.Value % 2 == 1)
                {
                    ((Image)this.rateAlbum.Children[i]).Source = imageHalfStar;
                }
            }
        }


        /// <summary>
        /// When the user clicks on a star, we update the album rate in database
        /// </summary>
        private void StarMouseDown(object sender, MouseButtonEventArgs e)
        {
            album.Rate = (byte)(starPos * 2);

            if (((Image)sender).Source == imageHalfStar)
            {
                album.Rate--;
            }

            // update database
            using (var context = new MusCatEntities())
            {
                context.Entry(album).State = System.Data.EntityState.Modified;
                context.SaveChanges();
            }
        }

        #endregion
    }


    //public static class AlbumCommands
    //{
    //    public static readonly RoutedUICommand SaveSong = new RoutedUICommand
    //            (
    //                    "Save Song",
    //                    "Save Song",
    //                    typeof(AlbumCommands),
    //                    new InputGestureCollection()
    //                            {
    //                                    new KeyGesture( Key.S, ModifierKeys.Control )
    //                            }
    //            );

    //    //Define more commands here, just like the one above
    //}
}