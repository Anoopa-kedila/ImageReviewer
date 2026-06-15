using ImageReviewerApp.Enums;

namespace ImageReviewerApp.Contracts;


public interface IImageOperationFactory
{
    IImageOperation CreateOperation(ImageOperation operation, object? parameters = null);

    IEnumerable<ImageOperation> GetAvailableOperations();
}
