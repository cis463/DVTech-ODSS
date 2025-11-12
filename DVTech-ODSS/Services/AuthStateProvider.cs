using DVTech_ODSS.Models;

namespace DVTech_ODSS.Services
{
    public class AuthStateProvider
    {
        private User? _currentUser;

        public event Action? OnAuthStateChanged;

        public User? CurrentUser => _currentUser;

        public bool IsAuthenticated => _currentUser != null;

        public bool IsAdmin => _currentUser?.Role == "Admin";

        public bool IsSecretary => _currentUser?.Role == "Secretary";

        public void SetUser(User? user)
        {
            _currentUser = user;
            OnAuthStateChanged?.Invoke();
        }

        public void Logout()
        {
            _currentUser = null;
            OnAuthStateChanged?.Invoke();
        }
    }
}
