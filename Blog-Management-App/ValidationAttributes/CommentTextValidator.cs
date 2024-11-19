using System.ComponentModel.DataAnnotations;

namespace Blog_Management_App.ValidationAttributes;

/*
 * will validate the content of comments to prevent inappropriate or prohibited language.
 * This Custom validation attribute will ensure that comments adhere to community guidelines by filtering out offensive or inappropriate language.
 * It enhances the quality of user-provided comments and maintains a respectful environment.
 */
public class CommentTextValidator:ValidationAttribute
{
    private readonly HashSet<string> _blackList;

    public CommentTextValidator()
    {
        _blackList = ["badword1", "badword2", "badword3"];
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var commentText = value as string;
        if (string.IsNullOrEmpty(commentText))
        {
            return ValidationResult.Success;
        }
        
        var words = commentText.ToLower().Split(" ",StringSplitOptions.RemoveEmptyEntries);
        foreach (var word in words)
        {
            if (_blackList.Contains(word))
            {
                return new ValidationResult($"The comment contains a prohibited word: {word}");
            }
        }
        
        return ValidationResult.Success;
    }
}