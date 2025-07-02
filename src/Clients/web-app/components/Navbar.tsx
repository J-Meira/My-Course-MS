"use client";

import { LoginButton, Logo, Search, UserDropdown } from "@/components";
import { useSession } from "next-auth/react";

export const Navbar = () => {
  const session = useSession();

  return (
    <header className="sticky top-0 z-50 flex justify-between bg-white p-5 items-center text-gray-800 shadow-md">
      <Logo />
      <Search />
      {session.data?.user ? (
        <UserDropdown user={session.data.user} />
      ) : (
        <LoginButton />
      )}
    </header>
  );
};
