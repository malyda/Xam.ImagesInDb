﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Media;
using TasteBeer.Database.Entity;
using Xamarin.Forms;

namespace ImagesInDb
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            List<BeerImage> beerImages2 = App.Database.GetBeerImagesAsync().Result;
            /*
            image.Source = Xamarin.Forms.ImageSource.FromStream(
              () => new MemoryStream(Convert.FromBase64String(beerImages2[0].Image)));
            */
            if (beerImages2.Count > 0) listviewWithImages.ItemsSource = beerImages2;

            takePhoto.Clicked += async (sender, args) =>
            {
                await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    DisplayAlert("No Camera", ":( No camera available.", "OK");
                    return;
                }

                var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Directory = "Sample",
                    Name = "test.jpg"
                });

                if (file == null)
                    return;

                await DisplayAlert("File Location", file.Path, "OK");

                BeerImage beer = new BeerImage();


                var stream2 = file.GetStream();

                System.IO.BinaryReader br = new System.IO.BinaryReader(stream2);
                Byte[] bytes = br.ReadBytes((Int32) stream2.Length);
                string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);

                beer.ImageRaw = base64String;

                await App.Database.SaveBeerImageAsync(beer);

                List<BeerImage> beerImages = await App.Database.GetBeerImagesAsync();
                    
            };


        }
    }
}
