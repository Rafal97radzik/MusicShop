using MusicShop.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MusicShop.DAL
{
    public class StoreInitializer : DropCreateDatabaseAlways<StoreContext>
    {
        protected override void Seed(StoreContext context)
        {
            SeedStoreData(context);
            base.Seed(context);
        }

        private void SeedStoreData(StoreContext context)
        {
            List<Genre> genres = new List<Genre>
            {
                new Genre() {GenreId=1, Name="Rock", IconFileName= "rock.png"},
                new Genre() {GenreId=2, Name="Metal", IconFileName= "metal.png"},
                new Genre() {GenreId=3, Name="Jazz", IconFileName= "jazz.png"},
                new Genre() {GenreId=4, Name="Hip Hop", IconFileName= "hiphop.png"},
                new Genre() {GenreId=5, Name="R&B", IconFileName= "rnb.png"},
                new Genre() {GenreId=6, Name="Pop", IconFileName= "pop.png"},
                new Genre() {GenreId=7, Name="Reggae", IconFileName= "reagge.png"},
                new Genre() {GenreId=8, Name="Alternative", IconFileName= "alternative.png"},
                new Genre() {GenreId=9, Name="Electronic", IconFileName= "electro.png"},
                new Genre() {GenreId=10, Name="Classical", IconFileName= "classics.png"},
                new Genre() {GenreId=11, Name="Inne", IconFileName= "other.png"},
                new Genre() {GenreId=12, Name="Promocje", IconFileName= "promos.png"},
            };

            genres.ForEach(g => context.Genres.Add(g));
            context.SaveChanges();

            List<Album> albums = new List<Album>
            {
                new Album() {AlbumId=1, ArtistName="The Reds", AlbumTitle="More Way", Price=99, CoverFileName="1.png", IsBestseller=true, DateAdded=new DateTime(2014, 2,1), GenreId=1},
                new Album() {AlbumId=2, ArtistName="Dillusion", AlbumTitle="All that nothing", Price=54, CoverFileName="2.png", IsBestseller=true, DateAdded=new DateTime(2013,8,15), GenreId=1},
                new Album() {AlbumId=3, ArtistName="Allfor", AlbumTitle="Golden suffering", Price=102, CoverFileName="3.png", IsBestseller=true, DateAdded=new DateTime(2014, 1, 5), GenreId=1 },
                new Album() {AlbumId=4, ArtistName="Stasik", AlbumTitle="Pole samo się nie orze", Price=25, CoverFileName="4.png", IsBestseller=true, DateAdded=new DateTime(2014, 3,11), GenreId=1},
                new Album() {AlbumId=5, ArtistName="McReal", AlbumTitle="Illusion", Price=28, CoverFileName="5.png", IsBestseller=false, DateAdded=new DateTime(2014, 4, 2), GenreId=1},
                new Album() {AlbumId=6, ArtistName="The Punks", AlbumTitle="Women Eater", Price=30, CoverFileName="6.png", IsBestseller=false, DateAdded=new DateTime(2014, 4, 2), GenreId=1},
                new Album() {AlbumId=7, ArtistName="EX Band", AlbumTitle="What", Price=35, CoverFileName="7.png", IsBestseller=false, DateAdded=new DateTime(2014, 4, 2), GenreId=2},
                new Album() {AlbumId=8, ArtistName="Jamaican Cowboy", AlbumTitle="IceTeam Quatanamera", Price=21, CoverFileName="8.png", IsBestseller=false, DateAdded=new DateTime(2014,4,2), GenreId=2},
                new Album() {AlbumId=9, ArtistName="Str8ts", AlbumTitle="Sneakers Only", Price=25, CoverFileName="9.png", IsBestseller=false, DateAdded=new DateTime(2014,4,2), GenreId=2},
            };

            albums.ForEach(a => context.Albums.Add(a));
            context.SaveChanges();
        }
    }
}