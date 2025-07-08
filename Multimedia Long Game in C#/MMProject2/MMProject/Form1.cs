using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace MMProject
{
    public class CActor
    {
        public int x, y;
        public List<Bitmap> imgs;
        public int frameIndex = 0;
        public bool facingLeft = true;
    }
    public class CAdvImg
    {
        public Rectangle rcDst, rcSrc;
        public List<Bitmap> imgs;
        public int iFrame;
        public int iFramesLeft = 0; 
        public bool isSquished = false; 
        public int moveDirection = 30;
        public int moveSpeed = 10; 
        public int hitCount = 0;
        public int hearts = 3;
    }
    
    public class Lvl1Img
    {
        public Bitmap img;
        public Rectangle rsDst;
        public Rectangle rsSrc;
    }

    public class Button
    {
        public int x, y, w, h;
    }

    public class Laser
    {
        public int x, y, w, h;
        public int speed = 15;
    }


    public partial class Form1 : Form
    {
        Bitmap off;
        Bitmap heartImage = new Bitmap("heart.jpg");
        int maxHealth = 8;
        int currentHealth = 8;
        int heartWidth = 45;  
        int heartHeight = 40; 
        int heartSpacing = 0; 

        List<CAdvImg> LBlocks = new List<CAdvImg>();
        List<CAdvImg> LCoins = new List<CAdvImg>();
        List<CAdvImg> LMushroomEnemies = new List<CAdvImg>();
        List<CAdvImg> LElevator = new List<CAdvImg>();
        List<CAdvImg> VerE = new List<CAdvImg>();
        List<CAdvImg> Tube = new List<CAdvImg>();
        List<CAdvImg> FBall = new List<CAdvImg>();
        List<CAdvImg> FlyEnemy = new List<CAdvImg>();
        List<CAdvImg> actorFireballs = new List<CAdvImg>();
        List<CAdvImg> bowserFireballs = new List<CAdvImg>();

        List<CAdvImg> Ld = new List<CAdvImg>();
        List<CAdvImg> Platforms = new List<CAdvImg>();
        List<CAdvImg> PurpleFireballs = new List<CAdvImg>();
        List<Lvl1Img> LLevel3Ground = new List<Lvl1Img>();
        List<CAdvImg> LBowser = new List<CAdvImg>();

        Laser flyingEnemyLaser = new Laser();
        Random rand = new Random();
        int laserShootTimer = 0;  
        int laserDuration = 2000; 
        int laserActiveTime = 0;  

        Bitmap bk = new Bitmap("menu0.jpeg");
        Bitmap lk = new Bitmap("loading.jpeg");
        Bitmap Level2 = new Bitmap("menu3.jpeg");


        Button pb = new Button();
        Button sb = new Button();
        Button cb = new Button();
        Button eb = new Button();
        Button bb = new Button();
        Timer tt = new Timer();

        Button pb2 = new Button();
        Button sb2 = new Button();
        Button cb2 = new Button();
        Button eb2 = new Button();
        Button bb2 = new Button();
        Button tt2 = new Button();

        List<Lvl1Img> parts = new List<Lvl1Img>();
        CActor actor = new CActor();
        Lvl1Img Level1;

        Button wmax = new Button();
        Button wmin = new Button();
        Button Is = new Button();
        Button Ds = new Button();

        Button l2 = new Button();

        int scroll = 5;
        int speed = 10;

        bool flagPlay = false;
        bool flagSettings = false;
        bool flagCredits = false;
        bool flagstart = false;

        bool flagJump = false;

        int actorImgWidth, actorImgHeight;

        int jumpStartY;
        bool jumpingUp = true;

        bool movingLeft = false;
        bool movingRight = false;
        bool mushroomOnce = false;

        int ctTick = 0;

        bool gameover = false;
        Bitmap Gameover = new Bitmap("GameOver.jpg");

        bool flagMakeMushroomOnce = true;
        int groundLevel;
        int gravity = 25; 


        bool isInElevator = false;
        int elevatorRiseSpeed = 10;
        int levelTransitionY = 0;
        bool levelTransitionComplete = false;

        bool fireballTouched = false;
        bool PurplefireballTouched = false;

        CAdvImg activeFireball = null;

        bool level2Completed = false;

        bool onLadder = false;
        bool isLevel1 = true;
        bool islevel3 = false;

        int ctBowserTick = 0;
        bool win = false;

        public Form1()
        {
            this.Load += Form1_Load;
            this.Paint += Form1_Paint;
            this.WindowState = FormWindowState.Maximized;
            this.MouseDown += Form1_MouseDown;
            this.KeyDown += Form1_KeyDown;
            tt.Tick += Tt_Tick;
            tt.Start();
            this.KeyUp += Form1_KeyUp;

        }
        
        void DrawPurpleFireBall()
        {
            if (Ld.Count == 0) return; 

            CAdvImg ladder = Ld[0];
            Rectangle ladderRect = ladder.rcDst;

            Bitmap fireImg = new Bitmap("purpleFire.png");

            CAdvImg purpleFire = new CAdvImg();
            purpleFire.imgs = new List<Bitmap> { fireImg };
            purpleFire.iFrame = 0;

            int scaledWidth = fireImg.Width / 10;
            int scaledHeight = fireImg.Height / 10;

            int fireX = ladderRect.X + (ladderRect.Width - scaledWidth) / 2;
            int fireY = ladderRect.Y - scaledHeight; 

            purpleFire.rcDst = new Rectangle(fireX, 0, scaledWidth, scaledHeight);
            purpleFire.rcSrc = new Rectangle(0, 0, fireImg.Width, fireImg.Height);

            PurpleFireballs.Clear(); 
            PurpleFireballs.Add(purpleFire);
        }

        void makeBowser()
        {
            CAdvImg pnn = new CAdvImg();
            pnn.imgs = new List<Bitmap>();
            pnn.iFrame = 0;
            for (int i = 1; i <= 7; i++)
            {
                Bitmap img = new Bitmap("bowser" + i + ".png");
                img.MakeTransparent(img.GetPixel(0, 0));
                pnn.imgs.Add(img);
            }
            int scaleFactor = 2; 
            pnn.rcDst = new Rectangle(this.ClientSize.Width / 2, this.ClientSize.Height / 2 - 150,
                                     pnn.imgs[0].Width * scaleFactor, pnn.imgs[0].Height * scaleFactor); pnn.rcSrc = new Rectangle(0, 0, pnn.imgs[0].Width, pnn.imgs[0].Height);

            LBowser.Add(pnn);

        }
       
        void DrawPlatForm()
        {
            if (Ld.Count == 0) return; 

            CAdvImg platform = new CAdvImg();
            platform.iFrame = 0;
            platform.imgs = new List<Bitmap>();
            Bitmap img = new Bitmap("Veri.png");
            platform.imgs.Add(img);

            int platformWidth = img.Width / 16;
            int platformHeight = img.Height / 16;

            Rectangle ladder = Ld[0].rcDst;

            int platformX = ladder.X + (ladder.Width / 2) - (platformWidth / 2);
            int platformY = ladder.Y - platformHeight;

            platform.rcDst = new Rectangle(platformX, platformY, platformWidth, platformHeight);
            platform.rcSrc = new Rectangle(0, 0, img.Width, img.Height); 

            Platforms.Clear(); 
            Platforms.Add(platform);
        }

        void DrawLadder()
        {
            if (VerE.Count < 3) return;

            CAdvImg pnn = new CAdvImg();
            pnn.iFrame = 0;
            pnn.imgs = new List<Bitmap>();
            Bitmap img = new Bitmap("Ladder.png");
            pnn.imgs.Add(img);

            int resizedWidth = img.Width / 2;
            int resizedHeight = img.Height / 2 - 50;

            int ladderX = (this.ClientSize.Width - resizedWidth) / 2;
            int ladderY = this.ClientSize.Height / 2 - resizedHeight / 2;

            pnn.rcDst = new Rectangle(ladderX, 60, resizedWidth, resizedHeight*4);
            pnn.rcSrc = new Rectangle(0, 0, img.Width, img.Height);

            Ld.Clear(); 
            Ld.Add(pnn);
        }


        void MoveHorizontalElevators()
        {
            if (!levelTransitionComplete) return;

            List<Rectangle> prevPositions = new List<Rectangle>();
            foreach (CAdvImg elevator in VerE)
            {
                prevPositions.Add(elevator.rcDst);
            }

            for (int i = 0; i < VerE.Count; i++)
            {
                CAdvImg elevator = VerE[i];
                elevator.rcDst.X += elevator.moveDirection * elevator.moveSpeed;

                if (elevator.rcDst.Right > this.ClientSize.Width)
                {
                    elevator.rcDst.X = this.ClientSize.Width - elevator.rcDst.Width;
                    elevator.moveDirection *= -1;
                }
                else if (elevator.rcDst.X < 0)
                {
                    elevator.rcDst.X = 0;
                    elevator.moveDirection *= -1;
                }
            }

            for (int i = 0; i < VerE.Count; i++)
            {
                for (int j = i + 1; j < VerE.Count; j++)
                {
                    if (CheckRectCollision(VerE[i].rcDst, VerE[j].rcDst))
                    {
                        VerE[i].rcDst.X = prevPositions[i].X;
                        VerE[j].rcDst.X = prevPositions[j].X;
                        VerE[i].moveDirection *= -1;
                        VerE[j].moveDirection *= -1;
                        break;
                    }
                }
            }

            Rectangle actorFoot = new Rectangle(actor.x, actor.y + actorImgHeight / 2,
                                              actorImgWidth / 2, 1);
            for (int i = 0; i < VerE.Count; i++)
            {
                if (CheckRectCollision(actorFoot, VerE[i].rcDst))
                {
                    int deltaX = VerE[i].rcDst.X - prevPositions[i].X;
                    actor.x += deltaX;

                    if (actor.x < 0) actor.x = 0;
                    if (actor.x > this.ClientSize.Width - actorImgWidth / 2)
                        actor.x = this.ClientSize.Width - actorImgWidth / 2;
                    break;
                }
            }
        }
        void DrawFLyingEnemy()
        {
            CAdvImg pnn = new CAdvImg();
            pnn.iFrame = 0;
            pnn.imgs = new List<Bitmap>();
            Bitmap img = new Bitmap("FLyingLeft.png");
            pnn.imgs.Add(img);
            Bitmap img1 = new Bitmap("FLyingRight.png");
            pnn.imgs.Add(img1);
            pnn.rcDst = new Rectangle(this.ClientSize.Width / 2 - 200, 100 , pnn.imgs[0].Width - 200, pnn.imgs[0].Height - 170);
            pnn.rcSrc = new Rectangle(0, 0, pnn.imgs[0].Width, pnn.imgs[0].Height);
            pnn.rcDst = new Rectangle(this.ClientSize.Width / 2 - 200, 100, pnn.imgs[1].Width - 200, pnn.imgs[1].Height - 170);
            pnn.rcSrc = new Rectangle(0, 0, pnn.imgs[1].Width, pnn.imgs[1].Height);
            pnn.moveDirection = -1;  
            pnn.moveSpeed = 10;
            FlyEnemy.Add(pnn);
        }

        void UpdateFlyingEnemy()
        {
            for (int i = 0; i < FlyEnemy.Count; i++)
            {
                CAdvImg enemy = FlyEnemy[i];

                enemy.rcDst.X += enemy.moveDirection * enemy.moveSpeed;

                if (enemy.rcDst.X <= 0)
                {
                    enemy.moveDirection = 1; 
                    enemy.iFrame = 1;        
                }
                else if (enemy.rcDst.X + enemy.rcDst.Width >= this.ClientSize.Width)
                {
                    enemy.moveDirection = -1; 
                    enemy.iFrame = 0;          
                }
            }
        }

        void DrawTube()
        {
            CAdvImg pnn = new CAdvImg();
            pnn.iFrame = 0;
            pnn.imgs = new List<Bitmap>();
            Bitmap img = new Bitmap("Dp.png");
            pnn.imgs.Add(img);
            pnn.rcDst = new Rectangle(this.ClientSize.Width / 4 * 3 - 180, this.ClientSize.Height / 2  +50+ 15, pnn.imgs[0].Width - 300, pnn.imgs[0].Height - 300);
            pnn.rcSrc = new Rectangle(0, 0, pnn.imgs[0].Width, pnn.imgs[0].Height);
            Tube.Add(pnn);
        }


        void DrawFireBall()
        {
            CAdvImg pnn = new CAdvImg();
            pnn.iFrame = 0;
            pnn.imgs = new List<Bitmap>();

            Bitmap img = new Bitmap("fire.png");

            int newWidth = img.Width / 11;
            int newHeight = img.Height / 11;

            Bitmap resizedImg = new Bitmap(img, new Size(newWidth, newHeight));
            pnn.imgs.Add(resizedImg);

            Rectangle rect = new Rectangle();
            rect.X = this.ClientSize.Width / 4 * 3 - 130;
            rect.Y = this.ClientSize.Height / 2 +50 - 5;
            rect.Width = newWidth;
            rect.Height = newHeight;

            pnn.rcDst = rect;

            pnn.rcSrc = new Rectangle(0, 0, resizedImg.Width, resizedImg.Height);

            FBall.Add(pnn);
        }
        void CheckFireballTouch()
        {
            if (fireballTouched || FBall.Count == 0) return;

            Rectangle actorRect = new Rectangle(actor.x - scroll, actor.y, actorImgWidth / 2, actorImgHeight / 2);
            CAdvImg fireball = FBall[0];
            Rectangle fireballRect = fireball.rcDst;

            if (CheckRectCollision(actorRect, fireballRect))
            {
                fireballTouched = true; 
            }
        }


        void CheckPurpleFireballTouch()
        {
            if (PurplefireballTouched || PurpleFireballs.Count == 0) return;

            Rectangle actorRect = new Rectangle(actor.x - scroll, actor.y, actorImgWidth / 2, actorImgHeight / 2);

            for (int i = 0; i < PurpleFireballs.Count; i++)
            {
                Rectangle pfRect = PurpleFireballs[i].rcDst;

                if (CheckRectCollision(actorRect, pfRect))
                {
                    PurpleFireballs.RemoveAt(i);
                    PurplefireballTouched = true;

                    break;
                }
            }
        }



        void ShootFireball(int direction)
        {
            if (actorFireballs.Count > 0) return;

            CAdvImg fb = new CAdvImg();
            fb.iFrame = 0;
            fb.imgs = new List<Bitmap>();
            Bitmap img = new Bitmap("fire.png");

            int newWidth = img.Width / 17;
            int newHeight = img.Height / 17;
            Bitmap resizedImg = new Bitmap(img, new Size(newWidth, newHeight));
            fb.imgs.Add(resizedImg);

            fb.rcDst = new Rectangle(actor.x - scroll + 20, actor.y, newWidth, newHeight);
            fb.rcSrc = new Rectangle(0, 0, resizedImg.Width, resizedImg.Height);

            fb.moveDirection = direction;
            actorFireballs.Add(fb);
        }
        void BowserShootFireball(int direction)
        {
            if (bowserFireballs.Count > 0) return;

            CAdvImg fb = new CAdvImg();
            fb.iFrame = 0;
            fb.imgs = new List<Bitmap>();
            Bitmap img = new Bitmap("fire.png");

            int newWidth = img.Width / 8;
            int newHeight = img.Height / 8;
            Bitmap resizedImg = new Bitmap(img, new Size(newWidth, newHeight));
            fb.imgs.Add(resizedImg);

            fb.rcDst = new Rectangle(LBowser[0].rcDst.X - newWidth, LBowser[0].rcDst.Y, newWidth, newHeight);
            fb.rcSrc = new Rectangle(0, 0, resizedImg.Width, resizedImg.Height);

            fb.moveDirection = direction;
            bowserFireballs.Add(fb);
        }

        void ShootPurpleFireball(int direction)
        {
            CAdvImg pf = new CAdvImg();
            pf.iFrame = 0;
            pf.imgs = new List<Bitmap>();
            Bitmap img = new Bitmap("purpleFire.png");

            int newWidth = img.Width / 17;
            int newHeight = img.Height / 17;
            Bitmap resizedImg = new Bitmap(img, new Size(newWidth, newHeight));
            pf.imgs.Add(resizedImg);

            int startX = actor.x - scroll + actorImgWidth / 4;
            int startY = actor.y + actorImgHeight / 4;

            pf.rcDst = new Rectangle(startX, startY, newWidth, newHeight);
            pf.rcSrc = new Rectangle(0, 0, resizedImg.Width, resizedImg.Height);
            pf.moveDirection = direction;

            PurpleFireballs.Add(pf);
        }


        void DrawHealthBar(Graphics g)
        {

            heartImage.MakeTransparent(heartImage.GetPixel(0, 0));

            int startX = this.ClientSize.Width-400;
            int startY = 20;

            for (int i = 0; i < currentHealth; i++)
            {
                Rectangle heartRect = new Rectangle(
                    startX + (i * (heartWidth + heartSpacing)),
                    startY,
                    heartWidth,
                    heartHeight);

                g.DrawImage(heartImage, heartRect);
            }
        }

        void DrawVerticalElv()
        {
            Random rnd = new Random();

            CAdvImg pnn1 = new CAdvImg();
            pnn1.iFrame = 0;
            pnn1.imgs = new List<Bitmap>();
            Bitmap img1 = new Bitmap("Veri.png");
            pnn1.imgs.Add(img1);
            pnn1.rcDst = new Rectangle(50, this.ClientSize.Height / 2 + 50 + 15, img1.Width / 16, img1.Height / 16);
            pnn1.rcSrc = new Rectangle(0, 0, img1.Width, img1.Height);
            pnn1.moveDirection = rnd.Next(0, 2) == 0 ? -1 : 1; 
            VerE.Add(pnn1);

            CAdvImg pnn2 = new CAdvImg();
            pnn2.iFrame = 0;
            pnn2.imgs = new List<Bitmap>();
            Bitmap img2 = new Bitmap("Veri.png");
            pnn2.imgs.Add(img2);
            pnn2.rcDst = new Rectangle(50 + img1.Width / 14 + 300, this.ClientSize.Height / 2 + 50 + 15,
                                      img2.Width / 16, img2.Height / 16);
            pnn2.rcSrc = new Rectangle(0, 0, img2.Width, img2.Height);
            pnn2.moveDirection = rnd.Next(0, 2) == 0 ? -1 : 1;
            VerE.Add(pnn2);

            CAdvImg pnn3 = new CAdvImg();
            pnn3.iFrame = 0;
            pnn3.imgs = new List<Bitmap>();
            Bitmap img3 = new Bitmap("Veri.png");
            pnn3.imgs.Add(img3);
            pnn3.rcDst = new Rectangle(50 + (img1.Width / 14 + 300) * 2, this.ClientSize.Height / 2 + 50 + 15,
                                      img3.Width / 16, img3.Height / 16);
            pnn3.rcSrc = new Rectangle(0, 0, img3.Width, img3.Height);
            pnn3.moveDirection = rnd.Next(0, 2) == 0 ? -1 : 1;
            VerE.Add(pnn3);
        }

        void TakeDamage(int amount)
        {
            currentHealth -= amount;
            if (currentHealth < 0)
            {
                currentHealth = 0;
                gameover = true;
            }
            else
            {
                gameover = false;

            }
        }

        void Heal(int amount)
        {
            currentHealth += amount;
            if (currentHealth > maxHealth) currentHealth = maxHealth;
        }
        bool CheckRectCollision(Rectangle rect1, Rectangle rect2)
        {
            int rect1Right = rect1.X + rect1.Width;
            int rect1Bottom = rect1.Y + rect1.Height;
            int rect2Right = rect2.X + rect2.Width;
            int rect2Bottom = rect2.Y + rect2.Height;

            if (rect1.X < rect2Right &&
                rect1Right > rect2.X &&
                rect1.Y < rect2Bottom &&
                rect1Bottom > rect2.Y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        bool CheckBlockCollisions(Rectangle actorRect)
        {
            for (int i = 0; i < LBlocks.Count; i++)
            {
                if (CheckRectCollision(actorRect, LBlocks[i].rcDst))
                {
                    return true;
                }
            }
            return false;
        }

        bool IsOnGround()
        {
            if (onLadder) return true;

            Rectangle actorFoot = new Rectangle(actor.x, actor.y + actorImgHeight / 2,
                                              actorImgWidth / 2, 1);

            if (levelTransitionComplete && !islevel3)
            {
                foreach (CAdvImg elevator in VerE)
                {
                    if (CheckRectCollision(actorFoot, elevator.rcDst))
                    {
                        return true;
                    }
                }
            }

            Rectangle footRect = new Rectangle(actor.x - scroll, actor.y + actorImgHeight / 2,
                                             actorImgWidth / 2, 1);
            if (CheckBlockCollisions(footRect))
            {
                return true;
            }

            if (islevel3 && actor.y >= groundLevel)
            {
                return true;
            }

            if (!levelTransitionComplete && !islevel3 && actor.y >= groundLevel)
            {
                return true;
            }

            return false;
        }


        void CheckCollisions()
        {
            Rectangle actorRect = new Rectangle(actor.x - scroll, actor.y, actorImgWidth / 2, actorImgHeight / 2);

            for (int i = 0; i < LMushroomEnemies.Count; i++)
            {
                CAdvImg enemy = LMushroomEnemies[i];

                if (!enemy.isSquished)
                {
                    Rectangle enemyRect = enemy.rcDst;

                    int actorBottom = actorRect.Y + actorRect.Height;
                    int enemyTop = enemyRect.Y;
                    int actorRight = actorRect.X + actorRect.Width;
                    int enemyLeft = enemyRect.X;
                    int enemyRight = enemyRect.X + enemyRect.Width;

                    if (actorBottom >= enemyTop &&
                        actorRect.Y < enemyTop &&
                        actorRight > enemyLeft &&
                        actorRect.X < enemyRight &&
                        !jumpingUp)
                    {
                        enemy.iFrame = 2;
                        enemy.isSquished = true;
                        enemy.iFramesLeft = 2;

                        jumpStartY = actor.y - 50;
                        jumpingUp = true;
                    }
                    else if (CheckRectCollision(actorRect, enemyRect) && enemy.iFramesLeft <= 0)
                    {
                        TakeDamage(1);
                        enemy.iFramesLeft = 60;
                    }
                }

                if (enemy.iFramesLeft > 0)
                {
                    enemy.iFramesLeft--;
                }
            }

            if (flyingEnemyLaser != null)
            {
                Rectangle laserRect = new Rectangle(flyingEnemyLaser.x - scroll, flyingEnemyLaser.y, flyingEnemyLaser.w, flyingEnemyLaser.h);

                if (CheckRectCollision(actorRect, laserRect))
                {
                    TakeDamage(1);
                    flyingEnemyLaser = null; 
                }
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left && (!isInElevator || !isLevel1))
            {
                movingLeft = false;
            }

            if (e.KeyCode == Keys.Right && (!isInElevator || !isLevel1))
            {
                movingRight = false;
            }

        }
        void MakeMushroomEnemy()
        {
            if (LMushroomEnemies.Count == 0 && flagMakeMushroomOnce)
            {
                flagMakeMushroomOnce = false;
                CAdvImg pnn;
                int xEnemy = this.ClientSize.Width / 2;
                int yEnemy = this.ClientSize.Height / 2 + 30;

                Bitmap sampleImg = new Bitmap("mushroomEnemyLeft.jpg");
                int enemyWidth = sampleImg.Width;
                int enemyHeight = sampleImg.Height;

                for (int i = 0; i < 10; i++)
                {
                    pnn = new CAdvImg();
                    pnn.imgs = new List<Bitmap>();
                    pnn.iFrame = 0;

                    Bitmap img = new Bitmap("mushroomEnemyLeft.jpg");
                    img.MakeTransparent(img.GetPixel(0, 0));

                    Bitmap img2 = new Bitmap("mushroomEnemyRight.jpg");
                    img2.MakeTransparent(img2.GetPixel(0, 0));

                    Bitmap img3 = new Bitmap("mushroomEnemyCompressed.jpg");
                    img3.MakeTransparent(img3.GetPixel(0, 0));

                    if ((img2.Width != enemyWidth || img2.Height != enemyHeight) || (img3.Width != enemyWidth || img3.Height != enemyHeight))
                    {
                        Bitmap resizedImg2 = new Bitmap(img2, new Size(enemyWidth, enemyHeight));
                        Bitmap resizedImg3 = new Bitmap(img3, new Size(enemyWidth, img3.Height));

                        pnn.imgs.Add(img);
                        pnn.imgs.Add(resizedImg2);
                        pnn.imgs.Add(resizedImg3);

                    }
                    else
                    {
                        pnn.imgs.Add(img);
                        pnn.imgs.Add(img2);
                        pnn.imgs.Add(img3);

                    }

                    pnn.rcDst = new Rectangle(xEnemy, yEnemy, enemyWidth / 3, enemyHeight / 3);
                    pnn.rcSrc = new Rectangle(0, 0, enemyWidth, enemyHeight);

                    LMushroomEnemies.Add(pnn);
                    xEnemy += enemyWidth;
                }
            }
        }
        void moveMushrooms()
        {
            for (int i = 0; i < LMushroomEnemies.Count; i++)
            {
                CAdvImg ptrav = LMushroomEnemies[i];

                if (!ptrav.isSquished)
                {
                    int enemyRight = ptrav.rcDst.X + ptrav.rcDst.Width;

                    if (ptrav.rcDst.X > actor.x)
                    {
                        ptrav.rcDst.X -= 5;
                        ptrav.iFrame = 0; 
                    }
                    if (enemyRight < actor.x)
                    {
                        ptrav.rcDst.X += 5;
                        ptrav.iFrame = 1; 
                    }
                }
            }
        }

        void MoveBlocksWithBackground()
        {
            for (int i = 0; i < LBlocks.Count; i++)
            {
                CAdvImg block = LBlocks[i];

                if (movingLeft)
                {
                    block.rcDst.X += scroll; 


                }
                else if (movingRight)
                {
                    block.rcDst.X -= scroll; 

                }
            }
            for (int i = 0; i < LCoins.Count; i++)
            {
                CAdvImg coin = LCoins[i];

                if (movingLeft)
                {
                    coin.rcDst.X += scroll; 


                }
                else if (movingRight)
                {
                    coin.rcDst.X -= scroll; 

                }
            }
        }
        void Level1CompleteCheck()
        {
            if (LMushroomEnemies.Count == 0 && !flagMakeMushroomOnce && LElevator.Count == 0)
            {
                isLevel1 = true;
                LBlocks.Clear();
                CAdvImg pnn = new CAdvImg();
                pnn.iFrame = 0;
                pnn.imgs = new List<Bitmap>();
                Bitmap img = new Bitmap("Elevator.jpeg");
                pnn.imgs.Add(img);
                pnn.rcDst = new Rectangle(this.ClientSize.Width - pnn.imgs[0].Width - 20,
                                          this.ClientSize.Height / 2 - pnn.imgs[0].Height / 2+5,
                                          200, 200);
                pnn.rcSrc = new Rectangle(0, 0, pnn.imgs[0].Width, pnn.imgs[0].Height);
                LElevator.Add(pnn);

                DrawTube();
                DrawFireBall();
                LCoins.Clear();

            }

            if (LElevator.Count > 0 && !isInElevator)
            {
                Rectangle actorRect = new Rectangle(actor.x - scroll, actor.y, actorImgWidth / 2, actorImgHeight / 2);
                if (CheckRectCollision(actorRect, LElevator[0].rcDst))
                {
                    isInElevator = true;
                    actor.x =( LElevator[0].rcDst.X + LElevator[0].rcDst.Width / 2) -(actor.imgs[0].Width/2)+10 ;
                    actor.y = (LElevator[0].rcDst.Y + LElevator[0].rcDst.Height / 2) ;


                }
            }
        }
        void MoveElevator()
        {
            if (isInElevator && !levelTransitionComplete)
            {
                LElevator[0].rcDst.Y -= elevatorRiseSpeed;
                actor.y -= elevatorRiseSpeed;
                levelTransitionY += elevatorRiseSpeed;

                if (levelTransitionY >= this.ClientSize.Height)
                {
                    isLevel1 = false;
                    levelTransitionComplete = true;
                    Level1 = parts[1];
                    actor.x = 50;
                    actor.y = this.ClientSize.Height / 2 + 10;
                    LElevator.Clear();
                    isInElevator = false;
                    levelTransitionY = 0;

                    DrawVerticalElv();
                }
            }
        }

        private void Tt_Tick(object sender, EventArgs e)
        {
            if (flagPlay)
            {
                if (levelTransitionComplete && !IsActorVisible())
                {
                    gameover = true;
                }

                if (!level2Completed)
                {
                    MoveHorizontalElevators();
                }
                CheckCoinCollection();

                Level1CompleteCheck();
                MoveElevator();
                UpdateFlyingEnemy();
                CheckFireballTouch();
                CheckPurpleFireballTouch();

                for (int i = actorFireballs.Count - 1; i >= 0; i--)
                {
                    CAdvImg fb = actorFireballs[i];

                    if (fb.moveDirection == 0)
                        fb.rcDst.X += 30;
                    else if (fb.moveDirection == 1) 
                        fb.rcDst.Y -= 30;
                    if (LBowser.Count > 0 && CheckRectCollision(fb.rcDst, LBowser[0].rcDst))
                    {
                        actorFireballs.RemoveAt(i);
                    }
                    if (fb.rcDst.X > this.ClientSize.Width || fb.rcDst.Y < 0 || fb.rcDst.Y > this.ClientSize.Height)
                    {
                        actorFireballs.Clear();
                        break; 
                    }
                }
                for (int i = PurpleFireballs.Count - 1; i >= 0; i--)
                {
                    CAdvImg pf = PurpleFireballs[i];
                    if (LBowser.Count > 0 && CheckRectCollision(pf.rcDst, LBowser[0].rcDst))
                    {
                        PurpleFireballs.RemoveAt(i);
                        LBowser[0].hitCount++;

                        if (LBowser[0].hitCount >= 8)
                        {
                            LBowser[0].iFrame = 6; 
                        }
                        else
                        {
                            LBowser[0].iFrame = 5; 
                            LBowser[0].rcDst.Y = LBowser[0].rcDst.Y + 30;
                        }
                    }
                }
                for (int i = bowserFireballs.Count - 1; i >= 0; i--)
                {
                    CAdvImg fb = bowserFireballs[i];

                        fb.rcDst.X -= 50;
                        fb.rcDst.Y += 30;

                    if (fb.rcDst.X > this.ClientSize.Width || fb.rcDst.Y < 0 || fb.rcDst.Y > this.ClientSize.Height)
                    {
                        bowserFireballs.Clear();
                        break; 
                    }
                }

                laserShootTimer++;
                if (laserShootTimer > rand.Next(50, 100) && FlyEnemy.Count > 0 && flyingEnemyLaser == null)
                {
                    CAdvImg flyingEnemy = FlyEnemy[0];
                    int laserX = flyingEnemy.rcDst.X + flyingEnemy.rcDst.Width / 2 - 2; 
                    int laserY = flyingEnemy.rcDst.Y + flyingEnemy.rcDst.Height / 2;    

                    flyingEnemyLaser = new Laser();
                    flyingEnemyLaser.x = laserX;
                    flyingEnemyLaser.y = flyingEnemy.imgs[0].Height - 225 + 155;
                    flyingEnemyLaser.w = 5;
                    flyingEnemyLaser.h = this.ClientSize.Height / 2 - 130;

                    laserShootTimer = 0; 
                }

                if (flyingEnemyLaser != null && laserShootTimer > 5)
                {
                    flyingEnemyLaser = null;
                }
                if (FlyEnemy.Count > 0)
                {
                    for (int j = FlyEnemy.Count - 1; j >= 0; j--)
                    {
                        CAdvImg flyingEnemy = FlyEnemy[j];
                        Rectangle flyingEnemyRect = flyingEnemy.rcDst;

                        for (int i = actorFireballs.Count - 1; i >= 0; i--)
                        {
                            CAdvImg fb = actorFireballs[i];
                            Rectangle fbRect = fb.rcDst;

                            if (CheckRectCollision(fbRect, flyingEnemyRect))
                            {
                                flyingEnemy.hitCount++; 
                                actorFireballs.RemoveAt(i); 

                                if (flyingEnemy.hitCount >= 3)
                                {
                                    flyingEnemy.rcDst.Y += 20;
                                    
                                }
                            }
                        }
                    }
                    if (FlyEnemy[0].hitCount >= 3)
                    {
                    
                        FlyEnemy[0].rcDst.Y += 20;
                        flyingEnemyLaser = null;
               
                        level2Completed = true;
                    }

                    
                }
                if (LBowser.Count > 0)
                {
                    for (int j = LBowser.Count - 1; j >= 0; j--)
                    {
                        CAdvImg bowser = LBowser[j];
                        Rectangle bowserEnemyRect = bowser.rcDst;

                        for (int i = bowserFireballs.Count - 1; i >= 0; i--)
                        {
                            CAdvImg fb = bowserFireballs[i];
                            Rectangle fbRect = fb.rcDst;

                            if (CheckRectCollision(fbRect, bowserEnemyRect))
                            {
                                bowser.hitCount++; 
                                TakeDamage(2);
                                bowserFireballs.RemoveAt(i);

                                if (bowser.hitCount >= 8)
                                {
                                    bowser.iFrame = 6;
                                }
                                else
                                {
                                    bowser.iFrame = 5;
                                    bowser.rcDst.Y = LBowser[0].rcDst.Y + 30;

                             
                                }
                            }
                        }
                    }
                    if (LBowser[0].hitCount >= 8)
                    {
                        LBowser[0].iFrame = 6;

                    }
                    if (LBowser[0].hitCount >= 9)
                    {
                        win = true;

                    }


                }

                if (level2Completed && Ld.Count == 0)
                {
                    DrawLadder();
                    DrawPlatForm();
                    DrawPurpleFireBall();
                }

                if (PurplefireballTouched)
                {
                
                    islevel3 = true;
                    LCoins.Clear();
                    actor.y = this.ClientSize.Height / 2 + 10;
                    actor.x = 100;
                    VerE.Clear();
                    LElevator.Clear();
                    Ld.Clear();
                    Platforms.Clear();
                    makeBowser();
                    ctBowserTick++;
                    if (ctBowserTick % 10 == 0 && LBowser[0].hitCount<8)
                    {
                        LBowser[0].iFrame = (LBowser[0].iFrame + 1) % 5;
                        LBowser[0].rcDst.Y = this.ClientSize.Height / 2 - 150;
                    }




                    for (int i = 0; i < PurpleFireballs.Count; i++)
                    {
                        CAdvImg pf = PurpleFireballs[i];

                        switch (pf.moveDirection)
                        {
                            case 0: pf.rcDst.Y -= 30; break; 
                            case 1: pf.rcDst.Y += 30; break; 
                            case 2: pf.rcDst.X += 30; break; 
                            case 3: pf.rcDst.X -= 30; break; 
                        }

                        if (pf.rcDst.X < 0 || pf.rcDst.X > this.ClientSize.Width ||
                            pf.rcDst.Y < 0 || pf.rcDst.Y > this.ClientSize.Height)
                        {
                            PurpleFireballs.RemoveAt(i);
                            i--;
                        }
                    }
                }


                if (!isInElevator)
                {
                    if (ctTick == 25)
                    {
                        flagstart = true;
                        if (LMushroomEnemies.Count == 0)
                        {
                            mushroomOnce = true;
                        }
                        ctTick = 0;
                    }
                    Level1CompleteCheck();
                    if (!IsOnGround() && !flagJump && !onLadder)
                    {
                        actor.y += gravity;

                
                        if (levelTransitionComplete)
                        {
                            Rectangle nextPosition = new Rectangle(actor.x, actor.y, actorImgWidth / 2, actorImgHeight / 2);
                            foreach (CAdvImg elevator in VerE)
                            {
                                if (CheckRectCollision(nextPosition, elevator.rcDst))
                                {
                                    actor.y = elevator.rcDst.Y - (actorImgHeight / 2);
                                    break;
                                }
                            }
                        }
                    }
                    ctTick++;
                    CheckCollisions();
                    if (movingLeft || movingRight)
                    {
                        MoveBlocksWithBackground();
                    }
                }
            }
            if (mushroomOnce && LMushroomEnemies.Count == 0)
            {
                MakeMushroomEnemy();
                mushroomOnce = false;
            }

            if (LMushroomEnemies.Count > 0)
            {
                moveMushrooms();
            }


            if (movingLeft)
            {
                actor.facingLeft = true;
                Rectangle nextPosition = new Rectangle(actor.x - speed - scroll, actor.y, actorImgWidth / 2, actorImgHeight / 2);

                Rectangle edgeCheck = new Rectangle(actor.x - scroll - 5, actor.y + 1, actorImgWidth / 2 + 5, 1);
                if (!CheckBlockCollisions(edgeCheck) && IsOnGround())
                {
                    flagJump = false;
                }
                if (!CheckBlockCollisions(nextPosition))
                {
                    actor.x -= speed;

                    int newX = Level1.rsSrc.X - scroll;
                    if (newX >= 0 && newX + Level1.rsSrc.Width <= Level1.img.Width)
                    {
                        Level1.rsSrc.X = newX;
                        parts[2].rsSrc.X = newX;
                        LLevel3Ground[0].rsSrc.X = newX;
                        if (LBowser.Count > 0)
                        {
                            LBowser[0].rcDst.X += scroll;
                        }

                    }


                    if (!flagJump)
                    {
                        actor.frameIndex++;

                        if (actor.frameIndex > 3)
                        {
                            actor.frameIndex = 0;
                        }
                    }
                }

                else
                {
                    actor.frameIndex = 8;
                }
            }

            else if (movingRight)
            {
                actor.facingLeft = false;
                Rectangle nextPosition = new Rectangle(actor.x + speed - scroll, actor.y, actorImgWidth / 2, actorImgHeight / 2);
               

                if (!CheckBlockCollisions(nextPosition))
                {
                    actor.x += speed;
                    int newX = Level1.rsSrc.X + scroll;
                    if (newX >= 0 && newX + Level1.rsSrc.Width <= Level1.img.Width)
                    {
                        Level1.rsSrc.X = newX;
                        parts[2].rsSrc.X = newX;
                        LLevel3Ground[0].rsSrc.X = newX;
                        if (LBowser.Count > 0)
                        {
                            LBowser[0].rcDst.X -= scroll;
                        }

                    }

                    if (!flagJump)
                    {
                        actor.frameIndex++;

                        if (actor.frameIndex < 4)
                        {
                            actor.frameIndex = 4;
                        }

                        if (actor.frameIndex > 7)
                        {
                            actor.frameIndex = 4;
                        }
                    }

                    else
                    {
                        actor.frameIndex = 9;
                    }
                }
            }

            else
            {

                if (!flagJump)
                {
                    if (actor.facingLeft == true)
                    {
                        actor.frameIndex = 0;
                    }

                    else
                    {
                        actor.frameIndex = 4;
                    }
                }
                else
                {
                    if (actor.facingLeft == true)
                    {
                        actor.frameIndex = 8;
                    }

                    else
                    {
                        actor.frameIndex = 9;
                    }
                }
            }

            if (flagJump)
            {
                Rectangle actorRect = new Rectangle(actor.x - scroll, actor.y, actorImgWidth / 2, actorImgHeight / 2);
                if (jumpingUp)
                {
                    /*
                    int newY = Level1.rsSrc.Y - scroll;
                    if (newY >= 0 && newY + Level1.rsSrc.Height <= Level1.img.Height)
                    {
                        Level1.rsSrc.Y = newY;
                    }
                    */
                    
                    Rectangle nextPosition = new Rectangle(actor.x - scroll, actor.y - speed, actorImgWidth / 2, actorImgHeight / 2);
                    if (!CheckBlockCollisions(nextPosition))
                    {
                        actor.y -= speed;
                        if (actor.y <= jumpStartY - 105)
                        {
                            jumpingUp = false;
                        }
                    }
                    else
                    {
                        jumpingUp = false;
                    }
                }
                else
                {
                    /*
                    int newY = Level1.rsSrc.Y + scroll;
                    if (newY >= 0 && newY + Level1.rsSrc.Height <= Level1.img.Height)
                    {
                        Level1.rsSrc.Y = newY;
                    }
                    */
                    Rectangle nextPosition = new Rectangle(actor.x - scroll, actor.y + speed, actorImgWidth / 2, actorImgHeight / 2);
                    if (!CheckBlockCollisions(nextPosition) &&actor.y<=groundLevel)
                    {
                        actor.y += speed;
                        if (levelTransitionComplete)
                        {
                            Rectangle footRect = new Rectangle(actor.x, actor.y + actorImgHeight / 2,
                                                            actorImgWidth / 2, 1);
                            foreach (CAdvImg elevator in VerE)
                            {
                                if (CheckRectCollision(footRect, elevator.rcDst))
                                {
                                    actor.y = elevator.rcDst.Y - (actorImgHeight / 2) - 5;
                                    flagJump = false;
                                    jumpingUp = false;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        flagJump = false;
                        jumpingUp = false;

                        for (int i = 0; i < LBlocks.Count; i++)
                        {
                            CAdvImg block = LBlocks[i];
                            if (CheckRectCollision(nextPosition, block.rcDst))
                            {
                                actor.y = block.rcDst.Y - (actorImgHeight / 2);
                                break;
                            }
                        }


                        if (actor.y >= groundLevel)
                        {
                            actor.y = groundLevel;
                            flagJump = false;

                            if (actor.facingLeft == true)
                            {
                                actor.frameIndex = 0;
                            }
                            else
                            {
                                actor.frameIndex = 4;
                            }
                        }
                    }
                }
            }

            DrawDubb(this.CreateGraphics());
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            onLadder = false;

            if (Ld.Count > 0)
            {
                CAdvImg ladder = Ld[0];
                Rectangle ladderRect = ladder.rcDst;

                if (actor.x + actorImgWidth / 2 > ladderRect.X - 10 &&
                    actor.x < ladderRect.X + ladderRect.Width + 10 &&
                    actor.y + actorImgHeight > ladderRect.Y &&
                    actor.y < ladderRect.Y + ladderRect.Height)
                {
                    onLadder = true;
                }
                else
                {
                    onLadder = false;
                }
            }

            int moveSpeed = 5;
            int climbSpeed = 5;

            if (onLadder)
            {
                CAdvImg ladder = Ld[0];
                Rectangle ladderRect = ladder.rcDst; 
                if (e.KeyCode == Keys.Up)
                {
                    actor.y -= climbSpeed;
                    if (actor.y < ladderRect.Y - actorImgHeight / 2)
                    {
                        actor.y = ladderRect.Y - actorImgHeight / 2;
                        onLadder = false;
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    actor.y += climbSpeed;
                    if (actor.y + actorImgHeight > ladderRect.Y + ladderRect.Height)
                    {
                        actor.y = ladderRect.Y + ladderRect.Height - actorImgHeight;
                        onLadder = false;
                    }
                }

                if (e.KeyCode == Keys.Left)
                {
                    actor.x -= speed;
                    actor.facingLeft = true;
                }
                else if (e.KeyCode == Keys.Right)
                {
                    actor.x += speed;
                    actor.facingLeft = false;
                }
            }
            else
            {
                if (e.KeyCode == Keys.Left && (!isInElevator || !isLevel1))
                {
                    movingLeft = true;
                }

                if (e.KeyCode == Keys.Right && (!isInElevator || !isLevel1))
                {
                    movingRight = true;
                }

                if (e.KeyCode == Keys.Space && !flagJump && (!isInElevator || !isLevel1))
                {
                    flagJump = true;
                    jumpingUp = true;
                    jumpStartY = actor.y;
                    actor.frameIndex = actor.facingLeft ? 8 : 9;
                }
            }

            if (fireballTouched)
            {
                if (e.KeyCode == Keys.Z)
                {
                    ShootFireball(0); //  fireball horizontal
                }
                else if (e.KeyCode == Keys.X)
                {
                    ShootFireball(1); //  fireball vertical
                }
            }

            if (PurplefireballTouched)
            {
                if (e.KeyCode == Keys.C)
                    ShootPurpleFireball(0); // Up

                else if (e.KeyCode == Keys.V)
                    ShootPurpleFireball(1); // Down

                else if (e.KeyCode == Keys.B)
                    ShootPurpleFireball(2); // Right

                else if (e.KeyCode == Keys.N)
                    ShootPurpleFireball(3); // Left
            }
            if(LBowser.Count>0 && LBowser[0].hitCount<8)
            {
                BowserShootFireball(3);

            }

            removeMushroom();
        }

        void removeMushroom()
        {
            for(int i=0;i<LMushroomEnemies.Count;i++)
            {
                CAdvImg ptrav = LMushroomEnemies[i];
                if(ptrav.iFrame==2)
                {
                    LMushroomEnemies.Remove(ptrav);
                }
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {

                if (e.X >= pb.x && e.X <= (pb.x + pb.w) &&
                    e.Y >= pb.y && e.Y <= (pb.y + pb.h))
                {
                    flagPlay = true;
                }

                if (e.X >= sb.x && e.X <= (sb.x + sb.w) &&
                    e.Y >= sb.y && e.Y <= (sb.y + sb.h))
                {
                    flagSettings = true;
                }

                if (e.X >= cb.x && e.X <= (cb.x + cb.w) &&
                    e.Y >= cb.y && e.Y <= (cb.y + cb.h))
                {
                    flagCredits = true;
                }

                if (e.X >= bb.x && e.X <= (bb.x + bb.w) &&
                    e.Y >= bb.y && e.Y <= (bb.y + bb.h))
                {
                    flagPlay = false;
                    flagSettings = false;
                    flagCredits = false;
                    flagstart = false;
                    gameover = false;
                }

                if (e.X >= eb.x && e.X <= (eb.x + eb.w) &&
                    e.Y >= eb.y && e.Y <= (eb.y + eb.h))
                {
                    this.Close();
                }

                if (e.X >= wmax.x && e.X <= (wmax.x + wmax.w) &&
                    e.Y >= wmax.y && e.Y <= (wmax.y + wmax.h))
                {
                    this.WindowState = FormWindowState.Maximized;
                }

                if (e.X >= wmin.x && e.X <= (wmin.x + wmin.w) &&
                    e.Y >= wmin.y && e.Y <= (wmin.y + wmin.h))
                {
                    this.WindowState = FormWindowState.Normal;
                }

                if (e.X >= Is.x && e.X <= (Is.x + Is.w) &&
                    e.Y >= Is.y && e.Y <= (Is.y + Is.h))
                {
                    speed += 5;
                }

                if (e.X >= Ds.x && e.X <= (Ds.x + Ds.w) &&
                    e.Y >= Ds.y && e.Y <= (Ds.y + Ds.h))
                {
                    speed -= 5;
                    if(speed<0)
                    {
                        speed = 5;
                    }
                }
            }

            DrawDubb(this.CreateGraphics());
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            DrawDubb(e.Graphics);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            off = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
            groundLevel = this.ClientSize.Height / 2 + 10;

            DrawPlayButton();
            DrawSettingsButton();
            DrawCreditsButton();
            DrawExitButton();
            DrawBacKButton();

            Lvl1Img pnn = new Lvl1Img();
            pnn.rsDst = new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height);
            pnn.rsSrc = new Rectangle(0, 0, this.ClientSize.Width / 3, this.ClientSize.Height - 340);
            pnn.img = new Bitmap("level1.jpeg");
            parts.Add(pnn);

            Lvl1Img pnn2 = new Lvl1Img();
            pnn2.rsDst = new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height);
            pnn2.rsSrc = new Rectangle(0, 0, this.ClientSize.Width / 3, this.ClientSize.Height - 340);
            pnn2.img = new Bitmap("menu3.jpeg");
            parts.Add(pnn2);

            Lvl1Img pnn3 = new Lvl1Img();
            pnn3.rsDst = new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height);
            pnn3.img = new Bitmap("menu2.jpeg");
            pnn3.rsSrc = new Rectangle(0, 0, this.ClientSize.Width /3, this.ClientSize.Height - 340);
            parts.Add(pnn3);


            Lvl1Img pnn4 = new Lvl1Img();
            pnn4.rsDst = new Rectangle(0, this.ClientSize.Height /2-10, this.ClientSize.Width, this.ClientSize.Height /2-10);
            pnn4.img = new Bitmap("Veri.png");
            pnn4.rsSrc = new Rectangle(0, 0, this.ClientSize.Width / 3, this.ClientSize.Height - 340);
            LLevel3Ground.Add(pnn4);

            Level1 = parts[0];

            DrawActor();
            DrawWindowMax();
            DrawWindowMin();
            DrawIncSpeed();
            DrawDecSpeed();
            DrawBlocks();
            DrawFLyingEnemy();
        }
        void DrawBlocks()
        {
            Bitmap blockImg = new Bitmap("Block.png");
            blockImg.MakeTransparent(blockImg.GetPixel(0, 0));

            int blockWidth = blockImg.Width / 20;  
            int blockHeight = blockImg.Height / 20;
            int groundY = this.ClientSize.Height / 2 + 10;

            LBlocks.Clear();
            LCoins.Clear();

            for (int i = 0; i < 5; i++)
            {
                CAdvImg block = new CAdvImg();
                block.imgs = new List<Bitmap>();
                blockImg = new Bitmap("Block.png");
                blockImg.MakeTransparent(blockImg.GetPixel(0, 0));
                block.imgs.Add(blockImg);

                block.iFrame = 0;
                block.rcDst = new Rectangle(500 + i * blockWidth, groundY - blockHeight, blockWidth, blockHeight);
                block.rcSrc = new Rectangle(0, 0, blockImg.Width, blockImg.Height);
                LBlocks.Add(block);

                if (i == 2)
                {
                    AddCoin(500 + i * blockWidth, groundY - blockHeight - 50, blockWidth, blockHeight);
                }
            }

           
            for (int i = 0; i < 3; i++)
            {
                CAdvImg block = new CAdvImg();
                block.imgs = new List<Bitmap>();
                blockImg = new Bitmap("Block.png");
                blockImg.MakeTransparent(blockImg.GetPixel(0, 0));
                block.imgs.Add(blockImg);
                block.iFrame = 0;
                block.rcDst = new Rectangle(500 + 500 + i * blockWidth, groundY - blockHeight * 2, blockWidth, blockHeight);
                block.rcSrc = new Rectangle(0, 0, blockImg.Width, blockImg.Height);
                LBlocks.Add(block);

                if (i == 1)
                {
                    AddCoin(500 + 500 + i * blockWidth, groundY - blockHeight * 2 - 50, blockWidth, blockHeight);
                }
            }

            for (int i = 0; i < 4; i++)
            {
                CAdvImg block = new CAdvImg();
                block.imgs = new List<Bitmap>();
                blockImg = new Bitmap("Block.png");
                blockImg.MakeTransparent(blockImg.GetPixel(0, 0));
                block.imgs.Add(blockImg);
                block.iFrame = 0;
                block.rcDst = new Rectangle(500 + 800 + i * blockWidth, groundY - blockHeight * 4, blockWidth, blockHeight);
                block.rcSrc = new Rectangle(0, 0, blockImg.Width, blockImg.Height);
                LBlocks.Add(block);

                if (i == 2)
                {
                    AddCoin(500 + 800 + i * blockWidth, groundY - blockHeight * 4 - 50, blockWidth, blockHeight);
                }
            }

            for (int i = 0; i < 2; i++)
            {
                CAdvImg block = new CAdvImg();
                block.imgs = new List<Bitmap>();
                blockImg = new Bitmap("Block.png");
                blockImg.MakeTransparent(blockImg.GetPixel(0, 0));
                block.imgs.Add(blockImg);
                block.iFrame = 0;
                block.rcDst = new Rectangle(500 + 1200 + i * blockWidth, groundY - blockHeight * 3, blockWidth, blockHeight);
                block.rcSrc = new Rectangle(0, 0, blockImg.Width, blockImg.Height);
                LBlocks.Add(block);

                if (i == 0)
                {
                    AddCoin(500 + 1200 + i * blockWidth, groundY - blockHeight * 3 - 50, blockWidth, blockHeight);
                }
            }

            for (int i = 0; i < 6; i++)
            {
                CAdvImg block = new CAdvImg();
                block.imgs = new List<Bitmap>();
                blockImg = new Bitmap("Block.png");
                blockImg.MakeTransparent(blockImg.GetPixel(0, 0));
                block.imgs.Add(blockImg);
                block.iFrame = 0;
                block.rcDst = new Rectangle(500 + 1500 + i * blockWidth, groundY - blockHeight * 2, blockWidth, blockHeight);
                block.rcSrc = new Rectangle(0, 0, blockImg.Width, blockImg.Height);
                LBlocks.Add(block);
            }
        }

        void AddCoin(int x, int y, int width, int height)
        {
            CAdvImg coin = new CAdvImg();
            coin.imgs = new List<Bitmap>();
            Bitmap coinImg = new Bitmap("coin.png");
            coinImg.MakeTransparent(coinImg.GetPixel(0, 0));
            coin.imgs.Add(coinImg);

            coin.iFrame = 0;
            coin.rcDst = new Rectangle(x, y, width, height);
            coin.rcSrc = new Rectangle(0, 0, coinImg.Width, coinImg.Height);
            LCoins.Add(coin);
        }
        void CheckCoinCollection()
        {
            Rectangle actorRect = new Rectangle(actor.x - scroll, actor.y, actorImgWidth / 2, actorImgHeight / 2);

            for (int i = LCoins.Count - 1; i >= 0; i--)
            {
                if (CheckRectCollision(actorRect, LCoins[i].rcDst))
                {
                    LCoins.RemoveAt(i);
                    Heal(1); 
                }
            }
        }

        void DrawIncSpeed()
        {
            Is.x = this.ClientSize.Width / 4 - 170;
            Is.y = this.ClientSize.Height / 4 * 3 - 30 + 50;
            Is.w = 50;
            Is.h = 50;
        }

        void DrawDecSpeed()
        {
            Ds.x = this.ClientSize.Width / 4 + 50 - 105;
            Ds.y = this.ClientSize.Height / 4 * 3 - 30 + 50;
            Ds.w = 50;
            Ds.h = 50;
        }

        void DrawWindowMax()
        {
            wmax.x = this.ClientSize.Width / 4 - 170;
            wmax.y = this.ClientSize.Height / 4 + 50;
            wmax.w = 50;
            wmax.h = 50;
        }

        void DrawWindowMin()
        {
            wmin.x = this.ClientSize.Width / 4 + 50;
            wmin.y = this.ClientSize.Height / 4 + 50;
            wmin.w = 50;
            wmin.h = 50;
        }

        void DrawPlayButton()
        {
            pb.w = 150;
            pb.h = 65;
            pb.x = this.ClientSize.Width / 2 - pb.w / 2;
            pb.y = this.ClientSize.Height / 4 + 50;
        }

        void DrawSettingsButton()
        {
            sb.w = 150;
            sb.h = 65;
            sb.x = this.ClientSize.Width / 2 - sb.w / 2;
            sb.y = pb.y + pb.h + 20;
        }

        void DrawCreditsButton()
        {
            cb.w = 150;
            cb.h = 65;
            cb.x = this.ClientSize.Width / 2 - cb.w / 2;
            cb.y = sb.y + sb.h + 20;
        }

        void DrawExitButton()
        {
            eb.w = 150;
            eb.h = 65;
            eb.x = this.ClientSize.Width / 2 - eb.w / 2;
            eb.y = cb.y + cb.h + 20;
        }

        void DrawBacKButton()
        {
            bb.x = 0;
            bb.y = 0;
            bb.w = 150;
            bb.h = 65;
        }

        void DrawActor()
        {
            int ct = 0;
            actor.x = 50;
            actor.y = this.ClientSize.Height / 2 + 10;
            actor.imgs = new List<Bitmap>();
            jumpStartY = actor.y;

            for (int i = 0; i < 10; i++)
            {
                Bitmap frame;

                if (i < 4)
                {
                    frame = new Bitmap("MC" + i + "L.jpeg");

                    if (i == 0)
                    {
                        actorImgWidth = frame.Width;
                        actorImgHeight = frame.Height;
                    }
                }

                else if (i > 3 && i < 8)
                {
                    frame = new Bitmap("MC" + ct + "R.jpeg");
                    ct++;
                }

                else if (i == 8)
                {
                    frame = new Bitmap("MCJumpLeft.jpeg");
                }

                else
                {
                    frame = new Bitmap("MCJumpRight.jpeg");
                }

                frame.MakeTransparent(frame.GetPixel(0, 0));
                actor.imgs.Add(frame);
            }
        }

        bool IsActorVisible()
        {
            if (levelTransitionComplete)
            {
                return actor.y + actorImgHeight / 2 > 0 &&
                       actor.y < this.ClientSize.Height;
            }
            else
            {
                return actor.x + actorImgWidth / 2 > 0 &&
                       actor.x < this.ClientSize.Width &&
                       actor.y + actorImgHeight / 2 > 0 &&
                       actor.y < this.ClientSize.Height;
            }
        }

        void DrawScene(Graphics g2)
        {
            g2.Clear(Color.White);

            g2.DrawImage(bk, 0, 0, this.ClientSize.Width, this.ClientSize.Height);

            if (!flagPlay && !flagCredits && !flagSettings)
            {
                string title = "OreBound";
                Font font = new Font("Arial", 48, FontStyle.Bold);
                Brush brush = Brushes.Black;
                Pen pen = new Pen(Color.Black, 3);
                SizeF textSize = g2.MeasureString(title, font);

                float x = (this.ClientSize.Width - textSize.Width) / 2;
                float y = (this.ClientSize.Height - textSize.Height) / 4 - 75;

                g2.DrawString(title, font, brush, x, y);
                g2.DrawRectangle(pen, x, y, 340, 75);

                string playText = "Play";
                Font btnFont = new Font("Arial", 25, FontStyle.Bold);
                Brush btnTextBrush = Brushes.White;
                Brush btnBrush = Brushes.Black;

                g2.FillRectangle(btnBrush, pb.x, pb.y, pb.w, pb.h);
                g2.DrawString(playText, btnFont, btnTextBrush, pb.x + 35, pb.y + 12);

                string settingsText = "Settings";
                g2.FillRectangle(btnBrush, sb.x, sb.y, sb.w, sb.h);
                g2.DrawString(settingsText, new Font("Arial", 22, FontStyle.Bold), btnTextBrush, sb.x + 15, sb.y + 15);

                string creditsText = "Credits";
                g2.FillRectangle(btnBrush, cb.x, cb.y, cb.w, cb.h);
                g2.DrawString(creditsText, new Font("Arial", 25, FontStyle.Bold), btnTextBrush, cb.x + 13, cb.y + 13);

                string exitText = "Exit";
                g2.FillRectangle(btnBrush, eb.x, eb.y, eb.w, eb.h);
                g2.DrawString(exitText, btnFont, btnTextBrush, eb.x + 35, eb.y + 12);
            }

            if (flagCredits)
            {
                string cred1 = "By: Mohammed Ammar & Erfan Nada";
                string cred2 = "Supervisor: Dr Ahmed Farouk";

                Font btnFont = new Font("Arial", 25, FontStyle.Bold);
                Brush btnTextBrush = Brushes.White;
                Brush btnBrush = Brushes.Black;
                Pen pen = new Pen(Color.Black, 3);

                g2.DrawString(cred1, btnFont, btnTextBrush, this.ClientSize.Width / 4 + 50, pb.y + 12);
                g2.DrawString(cred2, btnFont, btnTextBrush, this.ClientSize.Width / 4 + 50, pb.y + 52);
                g2.DrawRectangle(pen, this.ClientSize.Width / 4 + 50, pb.y + 12, pb.w + 440, pb.h + 12);
            }

            if (flagSettings)
            {
                string wind = "Choose window mode:";
                Font btnFont = new Font("Arial", 18, FontStyle.Bold);
                Brush btnTextBrush = Brushes.Black;
                Brush btnBrush = Brushes.Black;
                Brush b = Brushes.White;
                Pen pen = new Pen(Color.Black, 3);

                g2.DrawString(wind, btnFont, btnTextBrush, this.ClientSize.Width / 4 - 170, this.ClientSize.Height / 4);
                g2.DrawRectangle(pen, this.ClientSize.Width / 4 - 170, this.ClientSize.Height / 4, 270, 30);

                g2.FillRectangle(btnBrush, wmax.x, wmax.y, wmax.w, wmax.h);
                string max = "Max";
                string min = "Min";
                Font f = new Font("Arial", 15, FontStyle.Bold);
                g2.DrawString(max, f, b, wmax.x + 2, wmax.y + 13);

                g2.FillRectangle(btnBrush, wmin.x, wmin.y, wmin.w, wmin.h);
                g2.DrawString(min, f, b, wmin.x + 4, wmin.y + 13);


                string speed = "Player Speed:";
                Font btnFont2 = new Font("Arial", 18, FontStyle.Bold);
                Brush btnTextBrush2 = Brushes.Black;
                Brush btnBrush2 = Brushes.Black;
                Brush b2 = Brushes.White;
                Pen pen2 = new Pen(Color.Black, 3);

                g2.DrawString(speed, btnFont2, btnTextBrush2, this.ClientSize.Width / 4 - 170, this.ClientSize.Height / 4 * 3 - 30);
                g2.DrawRectangle(pen, this.ClientSize.Width / 4 - 170, this.ClientSize.Height / 4 * 3 - 30, 165, 30);

                g2.FillRectangle(btnBrush, Is.x, Is.y, Is.w, Is.h);
                string inc = "+5";
                string dec = "-5";
                g2.DrawString(inc, f, b, Is.x + 10, Is.y + 15);

                g2.FillRectangle(btnBrush, Ds.x, Ds.y, Ds.w, Ds.h);
                g2.DrawString(dec, f, b, Ds.x + 12, Ds.y + 15);

            }

            if (flagPlay && !flagstart)
            {
                g2.DrawImage(lk, 0, 0, this.ClientSize.Width, this.ClientSize.Height);

                string playText = "Loading...";
                Font btnFont = new Font("Arial", 25, FontStyle.Bold);
                Brush btnTextBrush = Brushes.White;
                Brush btnBrush = Brushes.Black;
                Pen p = new Pen(Color.Black, 3);

                g2.DrawRectangle(p, pb.x, pb.y, pb.w + 15, pb.h);
                g2.DrawString(playText, btnFont, btnTextBrush, pb.x, pb.y + 12);
            }

            if (flagPlay && flagstart)
            {
                if (!levelTransitionComplete)
                {
                    g2.DrawImage(parts[0].img, parts[0].rsDst, parts[0].rsSrc, GraphicsUnit.Pixel);
                }
                else if(levelTransitionComplete && !islevel3)
                {
                    g2.DrawImage(parts[1].img, new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height),
                                parts[1].rsSrc, GraphicsUnit.Pixel);
                }
                else if (islevel3)
                {


                    g2.DrawImage(parts[2].img, new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height),
                                parts[2].rsSrc, GraphicsUnit.Pixel);
                    g2.DrawImage(LLevel3Ground[0].img, LLevel3Ground[0].rsDst,
                                                    LLevel3Ground[0].rsSrc, GraphicsUnit.Pixel);
                    for (int i = 0; i < LBowser.Count; i++)
                    {
                        CAdvImg ptrav = LBowser[0];
                        g2.DrawImage(ptrav.imgs[ptrav.iFrame], ptrav.rcDst,ptrav.rcSrc,GraphicsUnit.Pixel);

                    }
                    for (int i = 0; i < bowserFireballs.Count; i++)
                    {
                        CAdvImg fb = bowserFireballs[i];
                        g2.DrawImage(fb.imgs[fb.iFrame], fb.rcDst, fb.rcSrc, GraphicsUnit.Pixel);
                    }

                }
                DrawHealthBar(g2);


                for(int i=0;i<LMushroomEnemies.Count;i++)
                {
                    CAdvImg ptrav = LMushroomEnemies[i];
                    if (ptrav.iFrame == 2)
                    {
                        ptrav.rcDst.Y = this.ClientSize.Height / 2 + 80;
                        g2.DrawImage(ptrav.imgs[ptrav.iFrame], ptrav.rcDst, ptrav.rcSrc, GraphicsUnit.Pixel);

                    }
                    else
                    {
                        g2.DrawImage(ptrav.imgs[ptrav.iFrame], ptrav.rcDst, ptrav.rcSrc, GraphicsUnit.Pixel);
                    }
                }
                for (int i = 0; i < LBlocks.Count; i++)
                {
                    CAdvImg ptrav = LBlocks[i];
                    g2.DrawImage(ptrav.imgs[ptrav.iFrame], ptrav.rcDst, ptrav.rcSrc, GraphicsUnit.Pixel);
                    
                }
                for (int i = 0; i < LCoins.Count; i++)
                {
                    CAdvImg ptrav = LCoins[i];
                    g2.DrawImage(ptrav.imgs[ptrav.iFrame], ptrav.rcDst, ptrav.rcSrc, GraphicsUnit.Pixel);

                }

                if (!levelTransitionComplete)
                {
                    for (int i = 0; i < LElevator.Count; i++)
                    {
                        CAdvImg ptrav = LElevator[i];
                        g2.DrawImage(ptrav.imgs[ptrav.iFrame], ptrav.rcDst, ptrav.rcSrc, GraphicsUnit.Pixel);

                    }
                }

                if (LMushroomEnemies.Count == 0 && !flagMakeMushroomOnce && !levelTransitionComplete)
                {
                    for (int i = 0; i < Tube.Count; i++)
                    {
                        CAdvImg ptrav = Tube[i];
                        g2.DrawImage(ptrav.imgs[ptrav.iFrame], ptrav.rcDst, ptrav.rcSrc, GraphicsUnit.Pixel);

                        if (!fireballTouched)
                        {
                            CAdvImg ptrav2 = FBall[i];
                            g2.DrawImage(ptrav2.imgs[ptrav2.iFrame], ptrav2.rcDst, ptrav2.rcSrc, GraphicsUnit.Pixel);
                        }
                    }

                }
                if (fireballTouched)
                {
                    for (int i = 0; i < actorFireballs.Count; i++)
                    {
                        CAdvImg fb = actorFireballs[i];
                        g2.DrawImage(fb.imgs[fb.iFrame], fb.rcDst, fb.rcSrc, GraphicsUnit.Pixel);
                    }
                }

                if (level2Completed)
                {
                    for (int i = 0; i < Ld.Count; i++)
                    {
                        CAdvImg fb = Ld[i];
                        g2.DrawImage(fb.imgs[fb.iFrame], fb.rcDst, fb.rcSrc, GraphicsUnit.Pixel);
                    }

                    for (int i = 0; i < Platforms.Count; i++)
                    {
                        CAdvImg fb = Platforms[i];
                        g2.DrawImage(fb.imgs[fb.iFrame], fb.rcDst, fb.rcSrc, GraphicsUnit.Pixel);
                    }

                    for (int i = 0; i < PurpleFireballs.Count; i++)
                    {
                        CAdvImg fb = PurpleFireballs[i];
                        g2.DrawImage(fb.imgs[fb.iFrame], fb.rcDst, fb.rcSrc, GraphicsUnit.Pixel);
                    }
                }
                if (PurplefireballTouched)
                {
                    for (int i = 0; i < PurpleFireballs.Count; i++)
                    {
                        CAdvImg fb = PurpleFireballs[i];
                        g2.DrawImage(fb.imgs[fb.iFrame], fb.rcDst, fb.rcSrc, GraphicsUnit.Pixel);
                    }
                }
                if (actor.imgs != null && actor.imgs.Count > 0)
                {

                    int drawWidth = actorImgWidth / 2;
                    int drawHeight = actorImgHeight / 2;

                    Rectangle destRect = new Rectangle(actor.x - scroll, actor.y, drawWidth, drawHeight);
                    g2.DrawImage(actor.imgs[actor.frameIndex], destRect);
                }
            }
            if(gameover)
            {
                g2.DrawImage(Gameover, 0, 0, this.ClientSize.Width, this.ClientSize.Height);
            }
            if(win)
            {
                Bitmap img = new Bitmap("win2.jpg");
                g2.DrawImage(img, 0, 0, this.ClientSize.Width, this.ClientSize.Height);

            }

            if (flagPlay || flagCredits || flagSettings || flagstart ||gameover)
            {
                string playText = "Main Menu";
                Font btnFont = new Font("Arial", 15, FontStyle.Bold);
                Brush btnTextBrush = Brushes.White;
                Brush btnBrush = Brushes.Black;
                Pen p = new Pen(Color.Black, 3);

                g2.DrawRectangle(p, bb.x, bb.y, bb.w - 25, bb.h - 15);
                g2.DrawString(playText, btnFont, btnTextBrush, bb.x + 5, bb.y + 12);
            }

            SolidBrush redBrush = new SolidBrush(Color.Red);

            if (levelTransitionComplete && !gameover)
            {
                for(int i=0; i<VerE.Count; i++)
                {
                    CAdvImg ptrav = VerE[i];
                    g2.DrawImage(ptrav.imgs[ptrav.iFrame], ptrav.rcDst, ptrav.rcSrc, GraphicsUnit.Pixel);
                }

                for (int i = 0; i < FlyEnemy.Count; i++)
                {
                    CAdvImg ptrav = FlyEnemy[i];
                    g2.DrawImage(ptrav.imgs[ptrav.iFrame], ptrav.rcDst, ptrav.rcSrc, GraphicsUnit.Pixel);
                }

                if (flyingEnemyLaser != null) { 

                    g2.FillRectangle(redBrush, flyingEnemyLaser.x, flyingEnemyLaser.y, flyingEnemyLaser.w, flyingEnemyLaser.h);
                }

            }


        }

        void DrawDubb(Graphics g)
        {
            Graphics g2 = Graphics.FromImage(off);
            DrawScene(g2);
            g.DrawImage(off, 0, 0);
        }
    }
}
