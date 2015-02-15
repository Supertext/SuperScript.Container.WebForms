using System;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using SuperScript.Configuration;
using SuperScript.Emitters;

namespace SuperScript.Container.WebForms
{
    /// <summary>
    /// This control can be used to relocate and emit its contents in a common location.
    /// </summary>
    public abstract class Container : PlaceHolder
    {
        #region Global Constants and Variables

        private bool _addLocationComments = true;
        internal string _emitterKey = Settings.Instance.DefaultEmitter.Key;

        #endregion


        #region Properties

        /// <summary>
        /// <para>Determines whether the emitted contents contain comments indicating the original location when in debug mode.</para>
        /// <para>The default value is TRUE.</para>
        /// </summary>
        public bool AddLocationComments
        {
            get { return _addLocationComments; }
            set { _addLocationComments = value; }
        }


        /// <summary>
        /// <para>Indicates which instance of IEmitter the content should be added to.</para>
        /// <para>If not specified then the contents will be added to the default implementation of <see cref="IEmitter"/>.</para>
        /// </summary>
        public string EmitterKey
        {
            get { return _emitterKey; }
            set { _emitterKey = value; }
        }


        /// <summary>
        /// Gets or sets an index in the collection at which the contents are to be inserted.
        /// </summary>
        public int? InsertAt { get; set; }

        #endregion


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PreRender += __PreRender;
        }


	    protected abstract void __PreRender(object sender, EventArgs e);


        /// <summary>
        /// Generates a comment block, formatted for the specific output type of the derived implementation.
        /// </summary>
        /// <param name="comment">The comment which should appear inside the formatted comment block.</param>
        /// <param name="startOnNewLine">Indicates whether a new line should be added before the start of the comment block.</param>
        /// <returns>A string containing the specified comment inside a formatted comment block.</returns>
        protected abstract string GenerateComment(string comment, bool startOnNewLine = true);


        /// <summary>
        /// Gets the relative path and filename of the control or page which is instantiating this control.
        /// </summary>
        /// <returns>A relative path and filename.</returns>
        protected string GetFileName()
        {
            // to display the path of the file that we've dynamically located this JavaScript from...
            return ((base.NamingContainer.BindingContainer).TemplateControl).AppRelativeVirtualPath;
        }


        /// <summary>
        /// Obtains the contents that have been passed into this <see cref="Container" /> control.
        /// </summary>
        /// <returns>A string containing the contents that were declared inside the current instance of this control.</returns>
        protected virtual string GetContents()
        {
            using (var stringWriter = new StringWriter())
            using (var writer = new HtmlTextWriter(stringWriter))
            {
                base.Render(writer);

                writer.Flush();

                // this StringBuilder contains the contents of this control
                var sb = stringWriter.GetStringBuilder();

                // add the original location comment
                if (!AddLocationComments)
                {
                    return sb.ToString();
                }

                var filename = GetFileName();
                var openMessage = GenerateComment("Located dynamically from " + filename);
                var closeMessage = GenerateComment("End of script from " + filename);
                return openMessage + sb + closeMessage;
            }
        }
    }
}