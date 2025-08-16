import React from 'react';
import { Link, useLocation } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

const Header = () => {
  const { user, logout } = useAuth();
  const location = useLocation();

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
        <div className="logo-icon">TM</div>
        <span className="logo-text">TrainMe</span>
      </div>

      {/* Navigation & User */}
      <div className="header-nav">
        {/* Navigation Menu */}
        <nav className="nav-menu">
          <Link
            to="/workout"
            className={`nav-item ${location.pathname === '/workout' ? 'active' : ''}`}
          >
            Workout
          </Link>
        </nav>

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
      </div>
    </header>
  );
};

export default Header;
