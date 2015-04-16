using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Scarab_Commando
{
    // A classe Projectile herda todas as características de Sprite
    
    public class Projectile: Sprite
    {
        // Constrói o projétil pedindo seu tipo como parâmetro
        public Projectile(string theType)
        { type = theType; }

        private bool shoot, shooting;
        // Ativada no início de cada tiro
        public bool Shoot
        {
            get { return shoot; }
            set { shoot = value; }
        }

        // Ativada durante cada tiro
        public bool Shooting
        {
            get { return shooting; }
            set { shooting = value; }

        }

        // Tipo do projétil
        private string type;
        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        // Som a ser executado
        private string cue;
        public string Cue
        {
            get { return cue; }
            set { cue = value; }
        }
        
       

       
    }
}
