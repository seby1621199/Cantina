import React, { useContext, useState, useEffect } from 'react';
import { Navigate } from 'react-router-dom';
import { AuthContext } from '../Contexts/AuthContext';

const ProtectedRoute = ({ component: Component, allowedRoles, ...rest }) => {
  const { isLoggedIn, userRoles } = useContext(AuthContext);
  const [loading, setLoading] = useState(true);
  const [redirectTo, setRedirectTo] = useState(null);

  useEffect(() => {
    const timer = setTimeout(() => {
      if (!isLoggedIn) {
        setRedirectTo('/login');
      } else {
        const hasPermission = allowedRoles.some(role => userRoles.includes(role));
        setRedirectTo(hasPermission ? null : '/unauthorized');
      }
      setLoading(false);
    }, 50);

    return () => clearTimeout(timer);
  }, [isLoggedIn, userRoles, allowedRoles]);

  if (loading) {
    return <div>Loading...</div>;
  }

  if (redirectTo) {
    return <Navigate to={redirectTo} />;
  }

  return <Component {...rest} />;
};

export default ProtectedRoute;
