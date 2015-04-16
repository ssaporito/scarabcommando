using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Scarab_Commando
{
    public class Target:Sprite
    {
        // Determina se o alvo está caindo
        private bool falling;
        public bool Falling
        {
            get { return falling; }
            set { falling = value; }
        }

        // Determina o tipo de alvo
        private string type;
        public string Type
        {
            get { return type; }
            set { type = value; }
        }
        // Construtor padrão da classe
        public Target()
        { }
        // Construtor usando o tipo de alvo como parâmetro
        public Target(string theType)
        { type = theType; }

       // Reseta o alvo baseado na dificuldade
        public void Reset(string difficulty)
        {
            // Determina um tamanho aleatório para o alvo 
            this.Scale = (float)new Random().NextDouble() + 0.4f;

            // Determina uma posição aleatória para o alvo, mas sempre no topo da tela
            this.Position = new Vector2(new Random().Next(10, 790), 0);

            // Determina velocidade e vida do alvo dependendo da dificuldade e do tipo de alvo
            #region Target Preparations
            switch (Type)
            {
                case "meteor":
                    {
                        switch (difficulty)
                        {
                            case "Easy":
                                {
                                    this.speed.X = new Random().Next(-0, 10);
                                    this.speed.Y = new Random().Next(100, 200);
                                    this.Life = 2;
                                    break;
                                }
                            case "Normal":
                                {
                                    this.speed.X = new Random().Next(-50, 50);
                                    this.speed.Y = new Random().Next(200, 400);
                                    this.Life = 3;
                                    break;
                                }
                            case "Hard":
                                {
                                    this.speed.X = new Random().Next(-100, 100);
                                    this.speed.Y = new Random().Next(300, 700);
                                    this.Life = 5;
                                    break;
                                }
                            case "Insane":
                                {
                                    this.speed.X = new Random().Next(-100, 100);
                                    this.speed.Y = new Random().Next(500, 1000);
                                    this.Life = 7;
                                    break;

                                }
                                
                        }
                        break;
            }
        case "genkidama":
            {
                switch (difficulty)
                {
                    case "Easy":
                        {
                            this.Scale = 1.5f;
                            this.Position = new Vector2(-130, -1500);
                            this.speed.X = 0;
                            this.speed.Y = new Random().Next(10, 20);
                            this.Life = 100;
                            break;
                        }
                    case "Normal":
                        {
                            this.Scale = 1.5f;
                            this.Position = new Vector2(-130, -1500);
                            this.speed.X = 0;
                            this.speed.Y = new Random().Next(20, 30);
                            this.Life = 300;
                            break;
                        }
                    case "Hard":
                        {
                            this.Scale = 1.5f;
                            this.Position = new Vector2(-130, -1500);
                            this.speed.X = 0;
                            this.speed.Y = new Random().Next(30, 50);
                            this.Life = 600;
                            break;
                        }
                    case "Insane":
                        {
                            this.Scale = 1.5f;
                            this.Position = new Vector2(-130, -1500);
                            this.speed.X =0;
                            this.speed.Y = new Random().Next(100, 300);
                            this.Life = 1000;
                            break;

                        }

                }
                break;
            }
    }
            #endregion
}
    }
}
