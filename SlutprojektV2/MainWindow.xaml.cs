using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Threading;

namespace SlutprojektV2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer gameTimer = new DispatcherTimer();

        Rect playerHitBox;
        Rect groundHitBox;
        Rect obstacleHitBox;
        //definerar bilderna som rektanglar

        bool jumping;
        //kollar om spelaren hoppar

        int force = 20;
        int speed = 5;
        //definierar kraften och hastigheten för spelaren

        Random rnd = new Random();

        bool gameOver;
        //kollar om spelet är över

        double spriteIndex = 0;

        ImageBrush playerSprite = new ImageBrush();
        ImageBrush backgroundSprite = new ImageBrush();
        ImageBrush obstacleSprite = new ImageBrush();
        //målar upp bilden

        int[] obstaclePosition = { 320, 310, 300, 305, 315 };
        //positionen för hindren

        int score = 0;
        //ny variabel för spelpoäng

        public MainWindow()
        {
            InitializeComponent();

            MyCanvas.Focus();
            //när spelet startar fokuserar den på MyCanvas elementet

            gameTimer.Tick += GameEngine;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            //startar upp timern och den "tickar" varje 20 millisekund

            backgroundSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/background.gif"));
            //tar bakgrundsbilden från den definierade mappen

            background.Fill = backgroundSprite;
            background2.Fill = backgroundSprite;
            //lägger in den definerade bilden som bakgrund i applikationen

            Startgame();
        }

        private void GameEngine(object sender, EventArgs e)
        {
            Canvas.SetLeft(background, Canvas.GetLeft(background) - 3);
            Canvas.SetLeft(background2, Canvas.GetLeft(background2) - 3);
            //hur snabbt bakgrunden rör sig i applikationen

            if (Canvas.GetLeft(background) < -1262)
            {
                Canvas.SetLeft(background, Canvas.GetLeft(background2) + background2.Width);
            }
            //får bakgrunden att loopa

            if (Canvas.GetLeft(background2) < -1262)
            {
                Canvas.SetLeft(background, Canvas.GetLeft(background2) + background2.Width);
            }
            //får bakgrunden att loopa

            Canvas.SetTop(player, Canvas.GetTop(player) + speed);
            //positionerar spelaren
            Canvas.SetLeft(obstacle, Canvas.GetLeft(obstacle) - 12);
            //positionerar hindren 

            scoreText.Content = "Score: " + score;

            playerHitBox = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width - 15, player.Height - 10);
            obstacleHitBox = new Rect(Canvas.GetLeft(obstacle), Canvas.GetTop(obstacle), obstacle.Width, obstacle.Height);
            groundHitBox = new Rect(Canvas.GetLeft(ground), Canvas.GetTop(ground), ground.Width, ground.Height);
            //definierar spelaren, hindren och marken som rektanglar och överlappas inte


            if (playerHitBox.IntersectsWith(groundHitBox))
            {
                //om spelaren rör marken
                speed = 0;

                Canvas.SetTop(player, Canvas.GetTop(ground) - player.Height);

                jumping = false;

                spriteIndex += .5;

                if (spriteIndex > 8)
                {
                    spriteIndex = 1;
                }

                RunSprite(spriteIndex);
                
            }

            if (jumping == true)
            {
                //hur spelaren reagerar när den hoppar
                speed = -9;

                force -= 1;
            }
            else
            {
                speed = 12;
            }

            if (force < 0)
            {
                //om kraften är mindre än 0, hoppar inte spelaren
                jumping = false;
            }

            if (Canvas.GetLeft(obstacle) < -50)
            {
                Canvas.SetLeft(obstacle, 950);

                Canvas.SetTop(obstacle, obstaclePosition[rnd.Next(0, obstaclePosition.Length)]);

                score += 1;

                //ser till att hindren blir återkommande
            }

            if (playerHitBox.IntersectsWith(obstacleHitBox))
            {
                gameOver = true;

                gameTimer.Stop();
                //om spelaren nuddar hindren avbryts spelet
            }

            if (gameOver == true)
            {
                obstacle.Stroke = Brushes.Black;
                obstacle.StrokeThickness = 1;

                player.Stroke = Brushes.Red;
                player.StrokeThickness = 1;
                //målar linjer runt spelaren och hindren

                scoreText.Content = "Score: " + score + " Press Enter to play again!";
            }
            else
            {
                player.StrokeThickness = 0;
                obstacle.StrokeThickness = 0;
                //linjerna syns inte när spelet är igång
            }

        }


        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            //tryck enter för att starta spelet
            if (e.Key == Key.Enter && gameOver == true)
            {
                Startgame();
            }
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space && jumping == false && Canvas.GetTop(player) > 260)
            {
                jumping = true;
                force = 15;
                speed = -12;

                playerSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/newRunner_02.gif"));//definierar hoppbilden
            }
            //när mellanslagsknappen släpps hoppar spelaren
        }

        private void Startgame()
        {
            //Startar speler funktionen

            Canvas.SetLeft(background, 0);
            Canvas.SetLeft(background2, 1262);
            //definierar startpositionen för bakgrunden/ default location

            Canvas.SetLeft(player, 110);
            Canvas.SetTop(player, 140);
            //definierar startpositionen för spelaren/ default location

            Canvas.SetLeft(obstacle, 950);
            Canvas.SetTop(obstacle, 310);
            //definierar startpositionen för hindren/ default location

            RunSprite(1);
            //information till switch case satsen

            obstacleSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/obstacle.png"));
            obstacle.Fill = obstacleSprite;
            //definierar vilken bild som ska användas som hinder

            jumping = false;
            gameOver = false;
            score = 0;

            scoreText.Content = "Score: " + score;

            gameTimer.Start();
        }
        /// <summary>
        /// Switchcase sats där metoden kollar vilket värde som går igenom 
        /// och använder den bilden för spelaren som har samma värde som variabeln
        /// </summary>
        /// <param name="i">en variabel som väljer bild</param>
        private void RunSprite(double i)
        {

            switch (i)
            {
                case 1:
                    playerSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/newRunner_01.gif"));
                    break;
                case 2:
                    playerSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/newRunner_02.gif"));
                    break;
                case 3:
                    playerSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/newRunner_03.gif"));
                    break;
                case 4:
                    playerSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/newRunner_04.gif"));
                    break;
                case 5:
                    playerSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/newRunner_05.gif"));
                    break;
                case 6:
                    playerSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/newRunner_06.gif"));
                    break;
                case 7:
                    playerSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/newRunner_07.gif"));
                    break;
                case 8:
                    playerSprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/newRunner_08.gif"));
                    break;
            }

            player.Fill = playerSprite;

        }
    }
}