namespace RPThreadTrackerV3.BackEnd.Models.Configuration
{
    /// <summary>
    /// Wrapper class for application settings that are secure in nature.
    /// </summary>
    public class SecureAppSettings
    {
        /// <summary>
        /// Gets or sets the SendGrid API key for sending emails.
        /// </summary>
        /// <value>
        /// The SendGrid API key.
        /// </value>
        public string SendGridAPIKey { get; set; }

        /// <summary>
        /// Gets or sets the email address which should be displayed in the "From" field of Forgot Password emails.
        /// </summary>
        /// <value>
        /// The "From" email address for Forgot Password emails.
        /// </value>
        public string ForgotPasswordEmailFromAddress { get; set; }

        /// <summary>
        /// Gets or sets the email address to which messages from the contact form should be sent.
        /// </summary>
        /// <value>
        /// The recipient email for the contact form.
        /// </value>
        public string ContactFormEmailToAddress { get; set; }

        /// <summary>
        /// Gets or sets the application settings related to document storage.
        /// </summary>
        /// <value>
        /// The application settings related to document storage.
        /// </value>
        public DocumentsAppSettings Documents { get; set; }
    }
}
