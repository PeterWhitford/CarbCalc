using System;
using Android.App;
using Android.Text;
using Android.Widget;
using Java.Lang;

namespace CarbCalc
{
    class GenericTextWatcher : Android.Text.ITextWatcher
    {
        EditText et_;
        Activity ac_;

        public GenericTextWatcher(EditText et, Activity ac)
        {
            et_ = et;
            ac_ = ac;
        }


        public IntPtr Handle
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void AfterTextChanged(IEditable s)
        {
            // here you can modifiy according to your self.
        }

        public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void OnTextChanged(ICharSequence s, int start, int before, int count)
        {
            throw new NotImplementedException();
        }
    }
}