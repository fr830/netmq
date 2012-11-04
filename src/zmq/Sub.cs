/*
    Copyright (c) 2007-2012 iMatix Corporation
    Copyright (c) 2009-2011 250bpm s.r.o.
    Copyright (c) 2007-2011 Other contributors as noted in the AUTHORS file

    This file is part of 0MQ.

    0MQ is free software; you can redistribute it and/or modify it under
    the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation; either version 3 of the License, or
    (at your option) any later version.

    0MQ is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Text;

public class Sub : XSub {

    public class SubSession :XSub.XSubSession {

        public SubSession(IOThread io_thread_, bool connect_,
                SocketBase socket_, Options options_, Address addr_):base(io_thread_, connect_, socket_, options_, addr_) {

        }

    }
    
    public Sub(Ctx parent_, int tid_, int sid_) :base(parent_, tid_, sid_){
			options.SocketType = ZmqSocketType.ZMQ_SUB;

        //  Switch filtering messages on (as opposed to XSUB which where the
        //  filtering is off).
        options.Filter = true;
    }

		protected override bool xsetsockopt(ZmqSocketOptions option_, Object optval_)
    {
			if (option_ != ZmqSocketOptions.ZMQ_SUBSCRIBE && option_ != ZmqSocketOptions.ZMQ_UNSUBSCRIBE)
			{
            ZError.errno = (ZError.EINVAL);
            return false;
        }

        byte[] val;
        
        if (optval_ is String)
            val =  Encoding.ASCII.GetBytes ((String)optval_);
        else if (optval_ is byte[])
            val = (byte[]) optval_;
        else
            throw new ArgumentException();
            
        //  Create the subscription message.
        Msg msg = new Msg(val.Length + 1);
				if (option_ == ZmqSocketOptions.ZMQ_SUBSCRIBE)
            msg.put((byte)1);
				else if (option_ == ZmqSocketOptions.ZMQ_UNSUBSCRIBE)
            msg.put((byte)0);
        msg.put (val,1);

        //  Pass it further on in the stack.
        bool rc = base.xsend (msg, 0);
        return rc;
    }

		protected override bool xsend(Msg msg_, ZmqSendRecieveOptions flags_)
    {
        //  Overload the XSUB's send.
        ZError.errno = (ZError.ENOTSUP);
        return false;
    }

    protected override bool xhas_out()
    {
        //  Overload the XSUB's send.
        return false;
    }


}
