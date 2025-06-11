import { getCurrentUser } from "@/services";
import { LoginButton, Logo, Search, UserDropdown } from "@/components";

export const Navbar = async () => {
  const user = await getCurrentUser();

  return (
    <header className="sticky top-0 z-50 flex justify-between bg-white p-5 items-center text-gray-800 shadow-md">
      <Logo />
      <Search />
      {user ? <UserDropdown user={user} /> : <LoginButton />}
    </header>
  );
};
