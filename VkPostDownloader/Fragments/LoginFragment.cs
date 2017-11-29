using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Lilarcor.Cheeseknife;
using VKontakte;

namespace VkPostDownloader
{
    public class LoginFragment : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.login_layout, container, false);
            Cheeseknife.Inject(this, view);
            return view;
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
            Cheeseknife.Reset(this);
        }

        [InjectOnClick(Resource.Id.sign_in_button)]
        void OnClickMyButton(object sender, EventArgs e)
        {
            VKSdk.Login(this, MainApplication.MyScopes);
        }

        public async override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            bool vkResult;
            var task = VKSdk.OnActivityResultAsync(requestCode, resultCode, data, out vkResult);
            if (!vkResult)
            {
                base.OnActivityResult(requestCode, resultCode, data);
            }

            try
            {
                await task;
                FragmentTransaction fragmentTx = this.FragmentManager.BeginTransaction();
                fragmentTx.Replace(Resource.Id.fragment_container, new FollowersFragment()).Commit();

            }
            catch (VKException ex)
            {
                Toast.MakeText(Application.Context, Resource.String.missed_access_token_message, ToastLength.Long).Show();             

                //TODO: log error
                Console.WriteLine("User didn't pass Authorization: " + ex);
            }
        }
    }
}