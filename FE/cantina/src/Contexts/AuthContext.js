import React, { createContext, useState, useEffect } from 'react';
import { jwtDecode } from 'jwt-decode';

export const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [userRoles, setUserRoles] = useState([]);
  const [token, setToken] = useState('');

  useEffect(() => {
    const storedToken = localStorage.getItem('token');
    if (storedToken) {
      setToken(storedToken);
      setIsLoggedIn(true);
      try {
        const decodedToken = jwtDecode(storedToken);
        const roles = decodedToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
        setUserRoles(Array.isArray(roles) ? roles : roles ? roles.split(',') : []);
      } catch (error) {
        console.error("Error decoding token", error);
        setUserRoles([]);
      }
    }
  }, []);

  const login = (token) => {
    localStorage.setItem('token', token);
    setIsLoggedIn(true);
    try {
      const decodedToken = jwtDecode(token);
      console.log(decodedToken);
      const roles = decodedToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
      setUserRoles(Array.isArray(roles) ? roles : roles ? roles.split(',') : []);
    } catch (error) {
      console.error("Error decoding token", error);
      setUserRoles([]);
    }
  };

  const logout = () => {
    localStorage.removeItem('token');
    setIsLoggedIn(false);
    setUserRoles([]);
  };
  console.log(userRoles)

  return (
    <AuthContext.Provider value={{ isLoggedIn, token, userRoles, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};
