.. # Links

==========
User Guide
==========

*umsg* is a convenience wrapper around Python's :py:mod:`logging` library,
primarily intended to help facilitate easier library logging, while it works
for general logging as well.


Basic Usage
===========

There are two methods by which *umsg* may be utilized. The first is directly via
module member functions, or as a mixin to add logging class member methods.

Module Level
------------
In its simplest form, *umsg* can be invoked directly without pre-initialization
in the same manner that the standard logging facilities can.

.. code-block:: python

  import umsg

  umsg.log('Logging has been initialized')


Class Mixin
-----------
For a library class, however, you may instead use the :py:class:`~umsg.mixins.LoggingMixin`
mixin to add logging directly as a class method.

.. code-block:: python

  from umsg.mixins import LoggingMixin

  class MyClass(LoggingMixin):
    def __init__(self):
      super().__init__()
      self.log('Logging has been initialized')


Logging Prefixes
================

One of the core features *umsg* provides is message prefixes. While something
similar can be accomplished in vanilla logging via specially crafted Format
strings, an important distinction between *umsg*'s implementation, and vanilla
logging is *umsg* prefixes are always optional. When using the format attributes
from vanilla logging, you must always provide the values or the message will not
be logged at all:

  If you choose to use these attributes in logged messages, you need to exercise some care. In the above example, for instance, the Formatter has been set up with a format string which expects ‘clientip’ and ‘user’ in the attribute dictionary of the LogRecord. **If these are missing, the message will not be logged** because a string formatting exception will occur. So in this case, you always need to pass the extra dictionary with these keys.

  While this might be annoying, this feature is intended for use in specialized circumstances, such as multi-threaded servers where the same code executes in many contexts, and interesting conditions which arise are dependent on this context (such as remote client IP address and authenticated user name, in the above example). In such circumstances, it is likely that specialized Formatters would be used with particular Handlers.

  -- https://docs.python.org/3/library/logging.html, emphasis added

Prefixes permit blocks of program execution to be called out within logs when
necessary, and programmatically, without requiring the prefix in all instances.
This is independent of the logging message format string, and the two may be used
simultaneously. This convenience feature isn't meant to replace the message
formatting that vanilla logging provides, rather it compliments it for a specific,
though common, use-case.

Configured Prefixes
-------------------

The logging prefix can be added to the module profile at any point using the
:py:mod:`umsg.set_attr` function. For instance, the given example using the
default prefix format:

.. code-block:: python

  umsg.set_attr('msg_prefix', 'main')
  umsg.log('How now brown cow')

would result in the following message being created, and sent to the registered
logging handlers:

.. code-block::

  "[main] How now brown cow"

This occurs before the logging formatter is applied, thus, we don't have issues
with needing to always supply a format parameter. In addition, we can change the
prefix format as required, either on-the-fly, or more likely, at the start of
our program.

.. code-block:: python

  umsg.set_attr('msg_prefix_format', '<{prefix}> ')
  umsg.set_attr('msg_prefix', 'main')
  umsg.log('How now brown cow')

Changing the previous example as above now gives us:

.. code-block::

  "<main> How now brown cow"


Inline Prefixes
---------------

Prefixes are evaluated per message, prior to sending the message on to the
logging formatter. Thus, every log message may have a custom prefix, however
unlikely this scenario may be. Inline prefixes override the configured prefix
at the time and only for the duration of the specific call.

.. code-block:: python

  umsg.set_attr('msg_prefix', 'main')
  umsg.log('How now brown cow', prefix='alt')
  umsg.log('How now brown cow')

The above scenario results in two distinct log messages:

.. code-block::

  "[alt] How now brown cow"
  "[main] How now brown cow"

This enables specific functions, classes, decorators, or other code segments to
identify themselves as necessary, without worrying about logging state, or even
logging formatting parameters. More importantly, this enables you to selectively
expose only the information you wish.

Take the following example, which exposes the function name of every logging caller:

.. code-block:: python

  import logging

  logging.basicConfig(format='%(levelname)s - %(funcName)s - %(message)s', level=logging.DEBUG)

  class Rocket:
    def __security_check(self):
      logging.debug('checking security')

    def engine_start(self):
      logging.info('starting engines')

In this example, all logging messages will have their calling function/method's
name exposed in the logs. This might be helpful for debugging, chances are you
don't want or need such information in your day-to-day application logs. You may
still want some contextual information though.

.. code-block:: python

  import umsg

  umsg.init(msg_format='%(levelname)s - %(message)s',
            msg_prefix_format='{prefix} - ',
            level=logging.INFO
           )

  class Rocket(LoggingMixin):
    def __security_check(self):
      self.log('checking security', level='debug', prefix=__security_check.__name__)

    def engine_start(self):
      self.log('starting engines')

The format of the log message in this case is identical to the previous example,
with the exception that we only print the method name for the debug message. A
better approach would be to create a utility to pull the function name from the
stack, but the idea remains the same. The use of prefixes doesn't obviate or
seek to replace useful logging formatters, rather, it provides a complimentary
function.


The basicConfig Conundrum
=========================

Pythhon provides a zero config logging option in the standard :py:mod:`logging`
library by directly invoking the logging methods. The defaults for using this
option are not particularly useful for libraries or module logging purposes,
being that :py:class:`~logging.StreamHandler` is the chosen default handler, and
the handler logs directly to the root logger.

The approach taken by *umsg* is slightly different. It too supports a zero config
option inspired by :py:mod:`logging`, and will initialize a logging handler with
a default configuration by simply calling the logging method. Where *umsg*
diverges is in what it instantiates, and where. Instead of initializing a :py:class:`~logging.StreamHandler`,
*umsg* defaults to the more library appropriate :py:class:`~logging.NullHandler`,
and initializes it on the *module* itself, not the root logger. Doing so is
important for several reasons. This is in point of fact a `recommended behavior <https://docs.python.org/3/howto/logging.html#configuring-logging-for-a-library>`_
for library logging, which honors the idea that "the configuration of handlers
is the prerogative of the application developer who uses your library" [#quote1]_.
Using the :py:class:`~logging.NullHandler` ensures we're not emitting logs the developer
may not want, need, or even be aware of. It also honors the `Zen of Python <https://www.python.org/dev/peps/pep-0020/>`_
aphorism that explicit is better than implicit, by requiring the developer to
be explicit about their logging.

By using the module level logger, we further isolate the library, or module,
logging from unintentionally mucking up the consuming application's logs.
Logging should be intentional by design. Blindly logging to the root logger
negates this. Lastly, *umsg* defaults to the :py:const:`logging.INFO` level. This
is for two reasons. First, libraries don't usually emit a lot of general messages,
and by isolating to the module, the developer is already required to deliberately
enabling library logs, so they should override with their own desired level.
Second, *umsg* does support generic application logging, which more often
requires general information messages by default. If the application so requires,
:py:const:`logging.DEBUG` can be easily enabled, though we shouldn't assume this
is required by default for all applications.

Enabling the :py:class:`~logging.StreamHandler`, if desired, is trivial:

.. code-block:: python

  import logging
  import umsg

  umsg.init()
  umsg.add_handler(logging.StreamHandler())
  umsg.log('Hello World')

Unlike the :py:func:`~logging.basicConfig` defaults, which are set at the :py:const:`logging.WARNING`
level, this log message will immediately be displayed at the *umsg* default level
of :py:const:`logging.INFO`.


.. [#quote1] See https://docs.python.org/3/howto/logging.html
