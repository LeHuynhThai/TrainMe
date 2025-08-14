import React from 'react';
import { useAuth } from '../contexts/AuthContext';

const Header = () => {
  const { user, logout } = useAuth();

  // Lấy chữ cái đầu của username để làm avatar
  const getInitials = (name) => {
    return name ? name.charAt(0).toUpperCase() : 'U';
  };

  const handleLogout = () => {
    if (window.confirm('Bạn có chắc chắn muốn đăng xuất?')) {
      logout();
    }
  };

  return (
    <header className="header">
      {/* Logo */}
      <div className="header-logo">
        <span>TrainMe</span>
      </div>

      {/* User Info */}
      {user && (
        <div className="header-user">
          <div className="user-info">
            <div className="user-avatar">
              {getInitials(user.userName)}
            </div>
            <span className="user-name">{user.userName}</span>
          </div>

          <button
            onClick={handleLogout}
            className="btn btn-danger"
          >
            Đăng xuất
          </button>
        </div>
      )}
    </header>
  );
};

export default Header;
