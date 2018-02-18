using System.Collections.Generic;
using System.Linq;

namespace Server.Models
{
    public class ErrorCode
    {
        public int Code { get; set; }
        public string FrenchDescription { get; set; }
        public string EnglishDescription { get; set; }
        public bool IsOnline { get; set; }

        public ErrorCode(int code, string frenchdesc, string englishdesc, bool isonline = true)
        {
            this.Code = code;
            this.FrenchDescription = frenchdesc;
            this.EnglishDescription = englishdesc;
            this.IsOnline = isonline;
        }

        public override string ToString()
        {
            return $"{this.Code};;{this.FrenchDescription};;{this.EnglishDescription}";
        }

        public static readonly HashSet<ErrorCode> AllCodes = new HashSet<ErrorCode>()
        {
            new ErrorCode(601, "La commande n'existe pas.","Command does not exist."),
            new ErrorCode(602, "La commande est mal formatée.","Command is malformed."),
            new ErrorCode(603, "L'utilisateur n'existe pas.","User does not exist."),
            new ErrorCode(604, "Le service demandé n'existe pas.","Requested service does not exist."),
            new ErrorCode(605, "L'utilisateur n'a pas accès à ce service.","Access is denied."),
            new ErrorCode(606, "Le service est en cours d'acceptation.","Service is still pending."),
            new ErrorCode(607, "Le mot de passe n'est pas bon.","Password incorrect."),
            new ErrorCode(608, "Accès autorisé.","Access granted."),
            new ErrorCode(609, "L'utilisateur est déjà enregistré.","User already registered."),
            new ErrorCode(610, "Le mot de passe n'a pas changé.","Password didn't change."),
            new ErrorCode(611, "Le mot de passe a bien changé.","Password successfully changed."),
            new ErrorCode(612, "L'utilisateur n'est pas actif.","User is not active."),

            new ErrorCode(701, "L'utilisateur existe déjà.", "User already exist.", false),
            new ErrorCode(702, "Le service existe déjà.", "Service already exist.", false),
            new ErrorCode(703, "Le service n'existe pas.", "Service does not exist.", false),
            new ErrorCode(704, "L'utilisateur n'a pas cet accès.", "User does not have this access.", false),
            new ErrorCode(705, "L'utilisateur n'a aucun service.", "User does not have any service.", false),
            new ErrorCode(706, "L'utilisateur a déjà cette accès.", "User already have this access.", false),
            new ErrorCode(707, "Le type de log doit être \"server\" ou \"client\"", "Log type must be \"server\" or \"client\"", false),
            new ErrorCode(708, "Le paramètre n'est pas un nombre", "Parameter is not a number", false),
            new ErrorCode(709, "Le nombre renseigné n'est pas valide.", "Given number is not valid", false),

            new ErrorCode(801, "Vous n'êtes pas le propriétaire du token.","You are not the token's owner."),
            new ErrorCode(802, "Le token n'est plus valide.","Token is not valid anymore."),
            new ErrorCode(803, "La donnée n'existe pas.","Data does not exist."),
            new ErrorCode(804, "La donnée existe.","Data exists."),
            new ErrorCode(805, "La donnée a été stockée.","Data has been stored."),
            new ErrorCode(806, "La donnée n'a pas pu être stockée.","Data couldn't be stored."),
            new ErrorCode(807, "La donnée a été supprimée.","Data has been deleted."),
            new ErrorCode(808, "La donnée n'a pas pu être supprimée.","Data couldn't be deleted."),
            new ErrorCode(809, "Toutes les données ont été supprimées.", "Datas have been deleted."),
            new ErrorCode(810, "Les données n'ont pas toutes été supprimées","All datas couldn't be deleted."),
        };

        public static string GetDescriptionFromCode(int code, string lang = "fr")
        {
            if (!AllCodes.Any(x => x.Code == code)) return null;
            ErrorCode error = AllCodes.FirstOrDefault(x => x.Code == code);
            return lang.Equals("fr") ? error.FrenchDescription : error.EnglishDescription;
        }
    }
}
