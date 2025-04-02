using System.ComponentModel.DataAnnotations;

namespace Datify.Shared.Models.Enum;

public enum DocumentType
{
    [Display(Name = "Select One")]

    SelectOne,
    [Display(Name = "Face Picture")]

    Face,
    [Display(Name = "National Identity Management Card")]

    NIN_NIMC,
    [Display(Name = "Proof of Address (Light, Water, Rent bill, Statement)")]

    UtilityBill,
    [Display(Name = "Other ID Card")]

    OtherIdCard,
    [Display(Name = "Car Documents")]

    CarDoc,
    [Display(Name = "National/International Passport")]

    Passport,
    [Display(Name = "Drivers License")]

    DriverLicense,
    [Display(Name = "Market Items")]

    MarketItem,

    [Display(Name = "Other Types of documents")]

    Others
}