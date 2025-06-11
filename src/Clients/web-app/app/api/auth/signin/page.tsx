import { DefaultsDisplay } from "@/components";

type Props = {
  searchParams: {
    callbackUrl: string;
  };
};

export const SignIn = ({ searchParams }: Props) => {
  return (
    <DefaultsDisplay
      callbackUrl={searchParams.callbackUrl}
      showLogin
      subtitle="Please click below to login"
      title="You need to be logged in to view this page"
    />
  );
};

export default SignIn;
