import { auth } from "@/auth";
import { AuthTest, Heading } from "@/components";

export default async function Session() {
  const session = await auth();

  return (
    <div>
      <Heading title="Session dashboard" />
      <div className="bg-red-100 border-2 p-4 border-red-400 mt-2">
        <h3 className="text-lg">Session data:</h3>
        <pre className="whitespace-pre-wrap break-all">
          {JSON.stringify(session, null, 2)}
        </pre>
      </div>
      <div className="mt-4">
        <AuthTest />
      </div>
    </div>
  );
}
