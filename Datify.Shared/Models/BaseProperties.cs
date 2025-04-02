using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Datify.Shared.Models;


public class BaseProperties
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  // ✅ Ensures auto-increment
    public long Id { get; set; }
    public DateTime DateInserted { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; }
    public DateTime DateModified { get; set; }
    private bool _wasActionDoneBySomeoneElse;

    public bool WasActionDoneBySomeoneElse
    {
        get => _wasActionDoneBySomeoneElse;
        set
        {
            _wasActionDoneBySomeoneElse = value;
            if (value)
            {
                WhenWasActionDone = DateTime.UtcNow;
            }
        }
    }
    public long? WhoDidTheAction { get; set; }
    public DateTime? WhenWasActionDone { get; set; }
    public long? WhichCommunityWasActionDoneFor { get; set; }

}