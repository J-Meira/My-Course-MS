import { DefaultsDisplay } from "@/components";

type Props = {
  searchParams: Promise<{
    callbackUrl: string;
  }>;
};

export default async function SignIn({ searchParams }: Props) {
  const { callbackUrl } = await searchParams;

  return (
    <DefaultsDisplay
      callbackUrl={callbackUrl}
      showLogin
      subtitle="Please click below to login"
      title="You need to be logged in to view this page"
    />
  );
}
