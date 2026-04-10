import axios from 'axios';
import { useForm } from 'react-hook-form';
import { type KeyboardEvent, useRef, useState } from 'react';
import { Button } from '@/components/ui/button.tsx';
import { FaArrowUp } from 'react-icons/fa';

type FormData = {
   prompt: string;
};

type ChatResponse = {
   message: string;
};

const Chatbot = () => {
   const [messages, setMessages] = useState<string[]>([]);
   const { register, handleSubmit, reset, formState } = useForm<FormData>({ mode: 'onChange' });
   const conversationId = useRef(crypto.randomUUID());

   const onSubmit = async ({ prompt }: FormData) => {
      reset();
      setMessages((prev) => [...prev, prompt]);

      const { data } = await axios.post<ChatResponse>('/api/chat', {
         prompt,
         conversationId: conversationId.current,
      });

      setMessages((prev) => [...prev, data.message]);
   };

   const onKeyDown = (e: KeyboardEvent<HTMLFormElement>) => {
      if (e.key === 'Enter' && !e.shiftKey) {
         e.preventDefault();
         handleSubmit(onSubmit)();
      }
   };

   return (
      <div>
         <div>
            {messages.map((message, index) => (
               <p key={index}>{message}</p>
            ))}
         </div>

         <form
            onSubmit={handleSubmit(onSubmit)}
            onKeyDown={onKeyDown}
            className="flex flex-col gap-2 items-end border-2 p-4 rounded-3xl"
         >
            <textarea
               {...register('prompt', {
                  required: true,
                  validate: (data) => data.trim().length > 0,
               })}
               className="w-full border-0 focus:outline-0 resize-none"
               placeholder="Ask anything"
               maxLength={1000}
            />
            <Button
               disabled={!formState.isValid}
               className="rounded-full w-9 h-9"
            >
               <FaArrowUp />
            </Button>
         </form>
      </div>
   );
};
export default Chatbot;
