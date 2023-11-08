namespace swzwij.APIManager
{
    /// <summary>
    /// Abstract base class for defining API request configurations.
    /// </summary>
    public abstract class APIRequest
    {
        /// <summary>
        /// Gets the URL of the API endpoint to be requested.
        /// </summary>
        public abstract string URL { get; }
    }
}